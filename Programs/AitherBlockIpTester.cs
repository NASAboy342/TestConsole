using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;

namespace TestConsole.Programs;

/// <summary>
/// Tests Aither IP block feature (LCG-9427).
///
/// Verifies that transaction endpoints are blocked from blacklisted IPs
/// (office IPs) but still work from internal server IPs.
///
/// Endpoints tested (based on production log analysis):
///   /seamless-game-provider-api/place-bet.aspx     (11.5M/day)
///   /seamless-game-provider-api/result-bet.aspx    (9.9M/day)
///   /seamless-game-provider-api/payoff.aspx        (1.4M/day)
///   /seamless-game-provider-api/cancel-bet.aspx    (150K/day)
///   /seamless-game-provider-api/void-bet.aspx      (514/day)
///   /game/place-order.aspx                         (399K/day)
///   /game/result-order.aspx                        (57K/day)
///   /game/save-game-result.aspx                    (57K/day)
///
/// Usage:
///   dotnet run --project TestConsole.csproj -- AitherBlockIpTester --aitherUrl=<URL> --testBlock=1 --testAllow=1
///
///   --aitherUrl  Base URL of Aither (e.g. https://demo-aither-wl.example.com)
///   --testBlock  0|1 = Whether to test blocked endpoints (from office IP)
///   --testAllow  0|1 = Whether to test allowed endpoints (from server IP, requires proxy or direct access)
/// </summary>
public class AitherBlockIpTester
{
    // Configuration
    private static readonly string DefaultAitherUrl = "https://demo-aither-wl.568winex.com";

    // The Office IPs that should be BLOCKED by InternalNetworkBlacklistIps setting
    private static readonly string[] OfficeIpBlacklist =
    {
        "10.1.1.251",  // Production blacklist
        "10.1.2.251",  // Production blacklist
        "10.1.3.251",  // Production blacklist
        "10.1.4.251",  // Production blacklist
        "10.7.21.251", // Demo/UAT blacklist
        "61.220.125.7" // Demo/UAT blacklist
    };

    // These are the endpoints that SHOULD be blocked from office IPs
    private readonly EndpointTest[] _transactionEndpoints = new EndpointTest[]
    {
        CreateEndpoint("SEAMLESS", "place-bet", "/web-root/restricted/v2/seamless-game-provider-api/place-bet.aspx", BuildPlaceBetRequest),
        CreateEndpoint("SEAMLESS", "result-bet", "/web-root/restricted/v2/seamless-game-provider-api/result-bet.aspx", BuildResultBetRequest),
        CreateEndpoint("SEAMLESS", "payoff", "/web-root/restricted/v2/seamless-game-provider-api/payoff.aspx", BuildPayoffRequest),
        CreateEndpoint("SEAMLESS", "cancel-bet", "/web-root/restricted/v2/seamless-game-provider-api/cancel-bet.aspx", BuildCancelBetRequest),
        CreateEndpoint("SEAMLESS", "void-bet", "/web-root/restricted/v2/seamless-game-provider-api/void-bet.aspx", BuildVoidBetRequest),
        CreateEndpoint("LEGACY",   "place-order", "/web-root/restricted/v2/game/place-order.aspx", BuildPlaceOrderRequest),
        CreateEndpoint("LEGACY",   "result-order", "/web-root/restricted/v2/game/result-order.aspx", BuildResultOrderRequest),
        CreateEndpoint("LEGACY",   "save-game-result", "/web-root/restricted/v2/game/save-game-result.aspx", BuildSaveGameResultRequest),
    };

    private static EndpointTest CreateEndpoint(string category, string name, string path, Func<HttpRequestMessage> buildRequest)
        => new(category, name, path, buildRequest);

    public async Task Run()
    {
        var args = Environment.GetCommandLineArgs().Skip(1).ToArray();
        var config = ParseArgs(args);

        Console.WriteLine();
        AnsiConsole.MarkupLine($"[bold]=== Aither IP Block Tester (LCG-9427) ===[/]");
        Console.WriteLine();

        if (string.IsNullOrEmpty(config.AitherUrl))
        {
            AnsiConsole.MarkupLine("[yellow]Using default Aither URL: {DefaultAitherUrl}[/]");
            config.AitherUrl = DefaultAitherUrl;
        }

        if (config.AitherUrl.StartsWith("https://"))
        {
            config.AitherUrl = config.AitherUrl.Substring(8);
        }
        if (config.AitherUrl.StartsWith("http://"))
        {
            config.AitherUrl = config.AitherUrl.Substring(7);
        }

        var baseUrl = config.AitherUrl;

        Console.WriteLine($"Aither URL: {baseUrl}");
        Console.WriteLine($"Block test: {config.TestBlock}");
        Console.WriteLine($"Allow test: {config.TestAllow}");
        Console.WriteLine();

        // Phase 1: Test that blocked IPs return 403
        if (config.TestBlock)
        {
            await TestBlockedEndpoints(baseUrl);
        }
        else
        {
            Console.WriteLine("Skipped blocked endpoint test (use --testBlock=1 to enable)");
        }

        // Phase 2: Test that allowed IPs work (requires actual server IP access)
        if (config.TestAllow)
        {
            await TestAllowedEndpoints(baseUrl);
        }
        else
        {
            Console.WriteLine("Skipped allowed endpoint test (use --testAllow=1 to enable)");
        }

        Console.WriteLine();
        AnsiConsole.MarkupLine("[bold green]=== Done ===[/]");
    }

    private async Task TestBlockedEndpoints(string baseUrl)
    {
        var table = new Table();
        table.AddColumn("Endpoint");
        table.AddColumn("Method");
        table.AddColumn("Expected");
        table.AddColumn("Actual");
        table.AddColumn("Time");
        table.AddColumn("Status");

        var results = new List<EndpointResult>();
        var sw = Stopwatch.StartNew();

        var tasks = _transactionEndpoints.Select(ep => Task.Run(async () =>
        {
            var swEp = Stopwatch.StartNew();

            var request = ep.BuildRequest();
            request.Headers.Add("X-Forwarded-For", "10.1.1.251"); // Simulate office IP

            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var response = await client.PostAsync($"https://{baseUrl}{ep.Path}", request.Content);

            swEp.Stop();
            var body = await response.Content.ReadAsStringAsync();

            return new EndpointResult
            {
                Endpoint = ep.Name,
                Category = ep.Category,
                Path = ep.Path,
                Expected = "BLOCK (403)",
                Actual = (int)response.StatusCode,
                Body = body,
                TimeMs = swEp.ElapsedMilliseconds,
                IsBlocked = response.StatusCode == HttpStatusCode.Forbidden
            };
        })).ToArray();

        var outcomes = await Task.WhenAll(tasks);
        results.AddRange(outcomes);

        sw.Stop();

        foreach (var result in results)
        {
            var statusColor = result.IsBlocked ? "green" : "red";
            var status = result.IsBlocked ? "✓ BLOCKED" : "✗ PASSED";
            var responseTime = $"{result.TimeMs}ms";

            var row = new[]
            {
                $"[[{result.Category}]] {result.Endpoint}",
                "POST",
                "403 Forbidden",
                $"{result.Actual}",
                responseTime,
                $"[{statusColor}]{status}[/]"
            };

            table.AddRow(row);
        }

        Console.WriteLine();
        AnsiConsole.MarkupLine("[bold]=== Blocked Endpoint Test Results ===[/]");
        AnsiConsole.Write(table);

        var blockedCount = results.Count(r => r.IsBlocked);
        var total = results.Count;
        Console.WriteLine();
        AnsiConsole.MarkupLine($"[bold]{blockedCount}/{total} endpoints correctly blocked from office IP[/]");

        if (blockedCount < total)
        {
            Console.WriteLine();
            AnsiConsole.MarkupLine("[red]FAIL: Some endpoints were NOT blocked. The IP filter is NOT working.[/]");
        }
        else
        {
            Console.WriteLine();
            AnsiConsole.MarkupLine("[green]SUCCESS: All transaction endpoints correctly blocked from office IP.[/]");
        }
    }

    private async Task TestAllowedEndpoints(string baseUrl)
    {
        Console.WriteLine();
        AnsiConsole.MarkupLine("[yellow]Testing from server IP (requires direct access to Aither from internal network)[/]");

        var table = new Table();
        table.AddColumn("Endpoint");
        table.AddColumn("Expected");
        table.AddColumn("Actual");
        table.AddColumn("Time");
        table.AddColumn("Status");

        var results = new List<EndpointResult>();

        foreach (var ep in _transactionEndpoints)
        {
            var request = ep.BuildRequest();
            // Don't set X-Forwarded-For — let it use real server IP

            var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
            var sw = Stopwatch.StartNew();
            var response = await client.PostAsync($"https://{baseUrl}{ep.Path}", request.Content);
            sw.Stop();

            var result = new EndpointResult
            {
                Endpoint = ep.Name,
                Category = ep.Category,
                Path = ep.Path,
                Expected = "200 OK",
                Actual = (int)response.StatusCode,
                Body = await response.Content.ReadAsStringAsync(),
                TimeMs = sw.ElapsedMilliseconds,
                IsAllowed = response.StatusCode == HttpStatusCode.OK
            };
            results.Add(result);
        }

        foreach (var result in results)
        {
            var statusColor = result.IsAllowed ? "green" : "yellow";
            var status = result.IsAllowed ? "✓ WORKS" : "✗ FAIL";

            table.AddRow(
                $"[[{result.Category}]] {result.Endpoint}",
                "200 OK",
                $"{result.Actual}",
                $"{result.TimeMs}ms",
                $"[{statusColor}]{status}[/]"
            );
        }

        AnsiConsole.MarkupLine("[bold]=== Allowed Endpoint Test Results ===[/]");
        AnsiConsole.Write(table);

        var allowedCount = results.Count(r => r.IsAllowed);
        var total = results.Count;
        Console.WriteLine();
        AnsiConsole.MarkupLine($"{allowedCount}/{total} endpoints working from server IP");
    }

    // ========== Request Builders ==========

    private static HttpRequestMessage BuildPlaceBetRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderID = 29,
            GPID = 29,
            PortfolioType = 9,
            Currency = 3,
            Orders = new[]
            {
                new
                {
                    PlayerID = "test_player_123",
                    RefNo = "TEST_PLACE_BET_001",
                    DealId = "TEST_DEAL_001",
                    BetCredit = 10.00m,
                    ProductType = 9,
                    GameType = 0,
                    BetCount = 1,
                    EventId = 0,
                    MatchNo = 0,
                    Odds = 0.0m,
                    Handicap = 0,
                    GameRoundID = "",
                    SeqNo = 0,
                    BetDetail = ""
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    private static HttpRequestMessage BuildResultBetRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderID = 29,
            GPID = 29,
            PortfolioType = 9,
            Currency = 3,
            Results = new[]
            {
                new
                {
                    RefNo = "TEST_PLACE_BET_001",
                    SeqNo = 0,
                    Result = 1,
                    BetCredit = 0.0m,
                    ErrorCode = 0
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    private static HttpRequestMessage BuildPayoffRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderId = 29,
            FundProviderId = 29,
            PortfolioType = 9,
            Payoffs = new[]
            {
                new
                {
                    RefNo = "TEST_PLACE_BET_001",
                    SeqNo = 0,
                    PayoffAmount = 20.0m
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    private static HttpRequestMessage BuildCancelBetRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderID = 29,
            GPID = 29,
            PortfolioType = 9,
            CancelDatas = new[]
            {
                new
                {
                    RefNo = "TEST_PLACE_BET_001",
                    SeqNo = 0,
                    DealId = "TEST_DEAL_001",
                    ErrorCode = 0
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    private static HttpRequestMessage BuildVoidBetRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderID = 29,
            GPID = 29,
            PortfolioType = 9,
            Orders = new[]
            {
                new
                {
                    RefNo = "TEST_PLACE_BET_001",
                    SeqNo = 0,
                    DealId = "TEST_DEAL_001"
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    private static HttpRequestMessage BuildPlaceOrderRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderID = 29,
            GPID = 29,
            PortfolioType = 9,
            Orders = new[]
            {
                new
                {
                    PlayerID = "test_player_123",
                    RefNo = "TEST_PLACE_ORDER_001",
                    DealId = "TEST_DEAL_001",
                    BetCredit = 10.00m,
                    ProductType = 9,
                    SeqNo = 0,
                    BetCount = 1
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    private static HttpRequestMessage BuildResultOrderRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderID = 29,
            GPID = 29,
            PortfolioType = 9,
            Results = new[]
            {
                new
                {
                    RefNo = "TEST_PLACE_ORDER_001",
                    SeqNo = 0,
                    DealId = "TEST_DEAL_001",
                    Result = 1,
                    ErrorCode = 0
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    private static HttpRequestMessage BuildSaveGameResultRequest()
    {
        var payload = new
        {
            CustomerID = 12345,
            ServerID = "001",
            GameProviderID = 29,
            GPID = 29,
            PortfolioType = 9,
            Results = new[]
            {
                new
                {
                    GameRoundID = "TEST_ROUND_001",
                    RefNo = "TEST_PLACE_ORDER_001",
                    SeqNo = 0,
                    Result = 1,
                    BetCredit = 0.0m
                }
            }
        };

        return CreatePostRequest("application/x-www-form-urlencoded", new FormUrlEncodedContent(
            CreateFormData(payload)
        ));
    }

    // ========== Helpers ==========

    private static Dictionary<string, string> CreateFormData(object payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        return new Dictionary<string, string>
        {
            { "request", json }
        };
    }

    private static HttpRequestMessage CreatePostRequest(string contentType, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/dummy");
        request.Content = content;
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        return request;
    }

    // ========== Model ==========

    private record EndpointResult
    {
        public string Category { get; set; }
        public string Endpoint { get; set; }
        public string Path { get; set; }
        public string Expected { get; set; }
        public int Actual { get; set; }
        public string Body { get; set; }
        public long TimeMs { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsAllowed { get; set; }
    }

    private record EndpointTest(
        string Category,
        string Name,
        string Path,
        Func<HttpRequestMessage> BuildRequest
    );

    // ========== Arg Parsing ==========

    private static AppConfig ParseArgs(string[] args)
    {
        var config = new AppConfig();
        foreach (var arg in args)
        {
            if (arg.StartsWith("--aitherUrl=") || arg.StartsWith("--url="))
            {
                config.AitherUrl = arg.StartsWith("--url=")
                    ? arg.Substring("--url=".Length)
                    : arg.Substring("--aitherUrl=".Length);
            }
            else if (arg.StartsWith("--testBlock="))
            {
                int.TryParse(arg.Substring("--testBlock=".Length), out var v);
                config.TestBlock = v != 0;
            }
            else if (arg.StartsWith("--testAllow="))
            {
                int.TryParse(arg.Substring("--testAllow=".Length), out var v);
                config.TestAllow = v != 0;
            }
        }
        return config;
    }

    private record AppConfig
    {
        public string AitherUrl { get; set; }
        public bool TestBlock { get; set; } = true;
        public bool TestAllow { get; set; } = false;
    }
}