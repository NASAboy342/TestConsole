
using System.Text;
using System.Text.Json;
using TestConsole.Helper;
using TestConsole.model;
using TestConsole.Programs;

namespace TestConsole;

public class Program
{
    private const string MiranaUrl = "https://ex-api-yy.xxttgg.com";
    private const string CompanyKey = "28C7CA01E5E9455698C31C562B19F995";
    private const string UmPageHint = "mainopia";
    private const string MarsUrl = "http://lcg-mars-d10.568winex.com";
    private const string MarsCompanyKey = "A13C891AF44A4152A30FBC3502EC78B5";
    private const string MarsServerId = "YongYuan";

    public static async Task Main()
    {
        var marsHelper = new MarsHelper(MarsUrl, MarsCompanyKey, MarsServerId);
        Console.WriteLine("Getting all game info from Mars...");
        var gameListResponse = await marsHelper.GetGameListAsync(1031, false);

        var results = new List<TestResult>();

        foreach (var gameInfo in gameListResponse.SeamlessGameProviderGames.Where(g => g.IsEnabled && g.GameId != 0))
        {
            if (gameInfo == null)
            {
                Console.WriteLine("No enabled game found to test.");
                continue;
            }

            var availableCurrencies = gameInfo.SupportedCurrencies
                .Where(c => !string.Equals(c, "TMP", StringComparison.OrdinalIgnoreCase))
                .ToList();

            var currency = availableCurrencies.Count > 0
                ? availableCurrencies[Random.Shared.Next(availableCurrencies.Count)]
                : null;

            if (string.IsNullOrEmpty(currency))
            {
                Console.WriteLine($"No testable currency found for GameId {gameInfo.GameId}.");
                results.Add(TestResult.Skipped(gameInfo.GameProviderId, gameInfo.GameId, string.Empty, string.Empty, "No testable currency found."));
                continue;
            }

            var username = $"Login{gameInfo.GameProviderId}Monitor{currency}";

            Console.WriteLine($"Testing login - GpId: {gameInfo.GameProviderId}, GameId: {gameInfo.GameId}, Currency: {currency}, Username: {username}");
            results.Add(await TestLogin(username, currency, gameInfo.GameProviderId, gameInfo.GameId));
        }

        PrintSummary(results);
    }

    private static void PrintSummary(List<TestResult> results)
    {
        var successCount = results.Count(r => r.Status == TestStatus.Success);
        var failCount = results.Count(r => r.Status == TestStatus.Fail);
        var exceptionCount = results.Count(r => r.Status == TestStatus.Exception);
        var skippedCount = results.Count(r => r.Status == TestStatus.Skipped);

        Console.WriteLine();
        Console.WriteLine("===== Test Summary =====");
        Console.WriteLine($"Total: {results.Count}, Success: {successCount}, Fail: {failCount}, Exception: {exceptionCount}, Skipped: {skippedCount}");

        var notSuccessful = results.Where(r => r.Status != TestStatus.Success).ToList();
        if (notSuccessful.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Failures / Skips:");
            foreach (var result in notSuccessful)
            {
                Console.WriteLine($"[{result.Status}] GpId: {result.GpId}, GameId: {result.GameId}, Currency: {result.Currency}, Username: {result.Username}, Reason: {result.Message}");
            }
        }

        Console.WriteLine("=========================");
    }

    private static async Task<TestResult> TestLogin(string username, string currency, int gpId, int gameId)
    {
        try
        {
            var loginRequest = new PlayerLoginRequest
            {
                Username = username,
                CompanyKey = CompanyKey
            };

            using var httpClient = new HttpClient();
            var loginContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");
            Console.WriteLine($"Before Call Mirana Login - GpId: {gpId}, Currency: {currency}, Username: {username}");
            var loginHttpResponse = await httpClient.PostAsync($"{MiranaUrl}/web-root/restricted/player/login.aspx", loginContent);
            var loginResponseBody = await loginHttpResponse.Content.ReadAsStringAsync();
            var loginResponse = JsonSerializer.Deserialize<PlayerLoginResponse>(loginResponseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (loginResponse?.Error == null || loginResponse.Error.Id != 0)
            {
                var message = $"Mirana Login Error - ErrorId: {loginResponse?.Error?.Id}, ErrorMsg: {loginResponse?.Error?.Msg}";
                Console.WriteLine($"[FAIL] {message} - GpId: {gpId}, GameId: {gameId}, Currency: {currency}, Username: {username}");
                return TestResult.Fail(gpId, gameId, currency, username, message);
            }

            var gameRedirectUrl = $"https:{loginResponse.Url}&gpid={gpId}&gameid={gameId}&lang=en&device=d";

            var handler = new HttpClientHandler { AllowAutoRedirect = false };
            using var redirectClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(28) };
            var enterGameResponse = await redirectClient.GetAsync(gameRedirectUrl);
            var statusCode = (int)enterGameResponse.StatusCode;
            var redirectLocation = enterGameResponse.Headers.Location?.ToString() ?? string.Empty;

            Console.WriteLine($"Game redirect response - GpId: {gpId}, GameId: {gameId}, Currency: {currency}, StatusCode: {statusCode}, RedirectUrl: {redirectLocation}");

            if (IsStuckAtXiangu(redirectLocation))
            {
                var message = $"Xiangu Redirect fail - RedirectUrl: {redirectLocation}";
                Console.WriteLine($"[FAIL] {message} - GpId: {gpId}, GameId: {gameId}, Currency: {currency}");
                return TestResult.Fail(gpId, gameId, currency, username, message);
            }

            if (IsErrorUtopia(redirectLocation))
            {
                var message = $"Login seems to be stuck at Mainopia - RedirectUrl: {redirectLocation}";
                Console.WriteLine($"[FAIL] {message} - GpId: {gpId}, GameId: {gameId}, Currency: {currency}");
                return TestResult.Fail(gpId, gameId, currency, username, message);
            }

            if (statusCode != 200 && statusCode != 302)
            {
                var message = $"HTTP Status Code {statusCode}";
                Console.WriteLine($"[FAIL] {message} - GpId: {gpId}, GameId: {gameId}, Currency: {currency}");
                return TestResult.Fail(gpId, gameId, currency, username, message);
            }

            Console.WriteLine($"[SUCCESS] Login test passed - GpId: {gpId}, GameId: {gameId}, Currency: {currency}, Username: {username}");
            return TestResult.Success(gpId, gameId, currency, username);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] GpId: {gpId}, GameId: {gameId}, Currency: {currency}, Username: {username}, Exception: {ex.Message}");
            return TestResult.Exception(gpId, gameId, currency, username, ex.Message);
        }
    }

    private static bool IsStuckAtXiangu(string redirectUrl)
    {
        return !string.IsNullOrEmpty(redirectUrl)
               && redirectUrl.Contains("//gp-")
               && !redirectUrl.Contains("&gameCode=")
               && !redirectUrl.Contains("&isOpenGamewithPopUp=")
               && !redirectUrl.Contains("TwelveLiveGameLobby?");
    }

    private static bool IsErrorUtopia(string redirectUrl)
    {
        if (string.IsNullOrEmpty(redirectUrl)) return true;
        if (redirectUrl.Contains("GameRedirect/SeamlessGame?SsoToken=", StringComparison.OrdinalIgnoreCase)) return true;

        var uri = new Uri(redirectUrl);
        return uri.Host.Contains(UmPageHint, StringComparison.OrdinalIgnoreCase);
    }
}

public class PlayerLoginRequest
{
    public string CompanyKey { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
    public string Portfolio { get; set; } = "SeamlessGame";
}

public class PlayerLoginResponse
{
    public string Url { get; set; } = string.Empty;
    public string ServerId { get; set; } = string.Empty;
    public PlayerLoginError? Error { get; set; }
}

public class PlayerLoginError
{
    public int Id { get; set; }
    public string Msg { get; set; } = string.Empty;
}

public enum TestStatus
{
    Success,
    Fail,
    Exception,
    Skipped
}

public class TestResult
{
    public TestStatus Status { get; init; }
    public int GpId { get; init; }
    public int GameId { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;

    public static TestResult Success(int gpId, int gameId, string currency, string username) =>
        new() { Status = TestStatus.Success, GpId = gpId, GameId = gameId, Currency = currency, Username = username };

    public static TestResult Fail(int gpId, int gameId, string currency, string username, string message) =>
        new() { Status = TestStatus.Fail, GpId = gpId, GameId = gameId, Currency = currency, Username = username, Message = message };

    public static TestResult Exception(int gpId, int gameId, string currency, string username, string message) =>
        new() { Status = TestStatus.Exception, GpId = gpId, GameId = gameId, Currency = currency, Username = username, Message = message };

    public static TestResult Skipped(int gpId, int gameId, string currency, string username, string message) =>
        new() { Status = TestStatus.Skipped, GpId = gpId, GameId = gameId, Currency = currency, Username = username, Message = message };
}
