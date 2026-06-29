using System.Text;
using System.Text.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using TestConsole.Helper;
using TestConsole.model;

namespace TestConsole.Programs;

public class YyGameLauncher
{
    private static readonly Dictionary<string, Dictionary<string, EnvironmentConfig>> RequestData = new()
    {
        ["demo"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://ex-api-demo-yy.568win.com/web-root/restricted/player/login.aspx",
                CompanyKey = "5C0AAB8BB7874164B08CAD39A71A2C8E",
                AvailableUsers = new List<AccountInfo>
                {
                    new AccountInfo { Username = "TigerTestNPR", Currency = "NPR" },
                    new AccountInfo { Username = "Sch1038LoginPlayerCOP", Currency = "COP" },
                    new AccountInfo { Username = "qatestSCR", Currency = "SCR" },
                    new AccountInfo { Username = "qatestFKP", Currency = "FKP" },
                    new AccountInfo { Username = "qatestchf", Currency = "CHF" },
                    new AccountInfo { Username = "qatestGHS", Currency = "GHS" },
                    new AccountInfo { Username = "qatestPYG", Currency = "PYG" },
                    new AccountInfo { Username = "GaryTestNGN", Currency = "NGN" },
                    new AccountInfo { Username = "qatestNZD", Currency = "NZD" },
                    new AccountInfo { Username = "qatestBWP", Currency = "BWP" },
                    new AccountInfo { Username = "qatestGMD", Currency = "GMD" },
                    new AccountInfo { Username = "qatestTZS", Currency = "TZS" },
                    new AccountInfo { Username = "CssgUatPlayerKRW", Currency = "KRW" },
                    new AccountInfo { Username = "qatestmmk", Currency = "MMK" },
                    new AccountInfo { Username = "qatestBAM", Currency = "BAM" },
                    new AccountInfo { Username = "qatestMRU", Currency = "MRU" },
                    new AccountInfo { Username = "qatestCUP", Currency = "CUP" },
                    new AccountInfo { Username = "CssgUatPlayerHKD", Currency = "HKD" },
                    new AccountInfo { Username = "qatestBBD", Currency = "BBD" },
                    new AccountInfo { Username = "qatestKES", Currency = "KES" },
                    new AccountInfo { Username = "qatestNAD", Currency = "NAD" },
                    new AccountInfo { Username = "qatestAZN", Currency = "AZN" },
                    new AccountInfo { Username = "qatestBIF", Currency = "BIF" },
                    new AccountInfo { Username = "qatestSRD", Currency = "SRD" },
                    new AccountInfo { Username = "qatestRSD", Currency = "RSD" },
                    new AccountInfo { Username = "TestPlayerPHP001", Currency = "PHP" },
                    new AccountInfo { Username = "CssgUatPlayerBDT", Currency = "BDT" },
                    new AccountInfo { Username = "qatestQAR", Currency = "QAR" },
                    new AccountInfo { Username = "qatestJMD", Currency = "JMD" },
                    new AccountInfo { Username = "qatestAFN", Currency = "AFN" },
                    new AccountInfo { Username = "qatestHNL", Currency = "HNL" },
                    new AccountInfo { Username = "qatestERN", Currency = "ERN" },
                    new AccountInfo { Username = "qatestUZS", Currency = "UZS" },
                    new AccountInfo { Username = "qatestMWK", Currency = "MWK" },
                    new AccountInfo { Username = "BrendorTestMYK", Currency = "MYK" },
                    new AccountInfo { Username = "qatestSAR", Currency = "SAR" },
                    new AccountInfo { Username = "qatestSYP", Currency = "SYP" },
                    new AccountInfo { Username = "qatestALL", Currency = "ALL" },
                    new AccountInfo { Username = "qatestsek", Currency = "SEK" },
                    new AccountInfo { Username = "qatestKGS", Currency = "KGS" },
                    new AccountInfo { Username = "qatestmxn", Currency = "MXN" },
                    new AccountInfo { Username = "qatestrub", Currency = "RUB" },
                    new AccountInfo { Username = "qatestLYD", Currency = "LYD" },
                    new AccountInfo { Username = "qatestGIP", Currency = "GIP" },
                    new AccountInfo { Username = "KHPlayerusd1", Currency = "CNY" },
                    new AccountInfo { Username = "qatestCRC", Currency = "CRC" },
                    new AccountInfo { Username = "CssgUatPlayerJPY", Currency = "JPY" },
                    new AccountInfo { Username = "qatestJOD", Currency = "JOD" },
                    new AccountInfo { Username = "qatestNIO", Currency = "NIO" },
                    new AccountInfo { Username = "khplayerthb", Currency = "THB" },
                    new AccountInfo { Username = "qatesteur", Currency = "EUR" },
                    new AccountInfo { Username = "qatestANG", Currency = "ANG" },
                    new AccountInfo { Username = "qatestKWD", Currency = "KWD" },
                    new AccountInfo { Username = "qatestLRD", Currency = "LRD" },
                    new AccountInfo { Username = "qatestAWG", Currency = "AWG" },
                    new AccountInfo { Username = "qatestVUV", Currency = "VUV" },
                    new AccountInfo { Username = "qatestBRL1", Currency = "BRL" },
                    new AccountInfo { Username = "qatestCDF", Currency = "CDF" },
                    new AccountInfo { Username = "qatestMGA", Currency = "MGA" },
                    new AccountInfo { Username = "qatestLBP", Currency = "LBP" },
                    new AccountInfo { Username = "qatestXCD", Currency = "XCD" },
                    new AccountInfo { Username = "JamestestMAD", Currency = "MAD" },
                    new AccountInfo { Username = "CssgUatPlayerMYR", Currency = "MYR" },
                    new AccountInfo { Username = "qatestDKK", Currency = "DKK" },
                    new AccountInfo { Username = "qatestLSL", Currency = "LSL" },
                    new AccountInfo { Username = "qatestGBP", Currency = "GBP" },
                    new AccountInfo { Username = "qatestAOA", Currency = "AOA" },
                    new AccountInfo { Username = "qatestDOP", Currency = "DOP" },
                    new AccountInfo { Username = "qatestMOP", Currency = "MOP" },
                    new AccountInfo { Username = "JamestestDZD", Currency = "DZD" },
                    new AccountInfo { Username = "ttomplayerGDA", Currency = "GDA" },
                    new AccountInfo { Username = "qatestBHD", Currency = "BHD" },
                    new AccountInfo { Username = "CssgUatPlayerBND", Currency = "BND" },
                    new AccountInfo { Username = "qatestTMT", Currency = "TMT" },
                    new AccountInfo { Username = "qatestSZL", Currency = "SZL" },
                    new AccountInfo { Username = "qatestaud", Currency = "AUD" },
                    new AccountInfo { Username = "qatestOMR", Currency = "OMR" },
                    new AccountInfo { Username = "qatestMZN", Currency = "MZN" },
                    new AccountInfo { Username = "qatestPAB", Currency = "PAB" },
                    new AccountInfo { Username = "ssplayerLKR1", Currency = "LKR" },
                    new AccountInfo { Username = "qatestRWF", Currency = "RWF" },
                    new AccountInfo { Username = "qatestaed", Currency = "AED" },
                    new AccountInfo { Username = "qatestEGP", Currency = "EGP" },
                    new AccountInfo { Username = "qatestTJS", Currency = "TJS" },
                    new AccountInfo { Username = "leoPlayer1", Currency = "IDR" },
                    new AccountInfo { Username = "qatestCZK", Currency = "CZK" },
                    new AccountInfo { Username = "qatestSDG", Currency = "SDG" },
                    new AccountInfo { Username = "qatestUCC", Currency = "UCC" },
                    new AccountInfo { Username = "qatestARS", Currency = "ARS" },
                    new AccountInfo { Username = "qatestBMD", Currency = "BMD" },
                    new AccountInfo { Username = "ssplayerTRY", Currency = "TRY" },
                    new AccountInfo { Username = "qatestlak", Currency = "LAK" },
                    new AccountInfo { Username = "qatestDJF", Currency = "DJF" },
                    new AccountInfo { Username = "qatestKMF", Currency = "KMF" },
                    new AccountInfo { Username = "qatestYER", Currency = "YER" },
                    new AccountInfo { Username = "ssplayerPKR", Currency = "PKR" },
                    new AccountInfo { Username = "qatestCVE", Currency = "CVE" },
                    new AccountInfo { Username = "qatestGEL", Currency = "GEL" },
                    new AccountInfo { Username = "Sch1038LoginPlayerVES", Currency = "VES" },
                    new AccountInfo { Username = "qatestSLL", Currency = "SLL" }
                }
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://ex-api-demo-yy.568win.com/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["yy"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.xxttgg.com/web-root/restricted/player/login.aspx",
                CompanyKey = "ADAB03974FCD4CCFBD359ECFBC3AA7D2"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.xxttgg.com/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["yy2"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy-prod-c171.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "356927B85652440688C5A16F0B6D16A6"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy-prod-c171.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["yy4"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy-prod-c111.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "8FEF9C85FBE646CDB6807C852B99C38D"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy-prod-c111.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["yy5"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy-prod-c121.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "FC1DAF77566A47B78745D18609A76B75"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy-prod-c121.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["yy7"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy7-prod-a01.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "A13C891AF44A4152A30FBC3502EC78B5"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://mirana-yy7-prod-a01.568winex.com/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["dr_yy1"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy1.online/web-root/restricted/player/login.aspx",
                CompanyKey = "97092789561041FAA99D4722608AC1BF"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy1.online/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["dr_yy2"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy2.online/web-root/restricted/player/login.aspx",
                CompanyKey = "497447DE197B430C97E8389F42BE22B1"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy2.online/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["dr_yy4"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy4.online/web-root/restricted/player/login.aspx",
                CompanyKey = "CB1E2EE0847D4BB8BF603E6F05010B50"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy4.online/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        },
        ["dr_yy5"] = new Dictionary<string, EnvironmentConfig>
        {
            ["qa"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy5.online/web-root/restricted/player/login.aspx",
                CompanyKey = "981F8CABF7EF45DAADC9A1069BF0F23E"
            },
            ["aegina"] = new EnvironmentConfig
            {
                Url = "https://ex-api-yy.dryy5.online/web-root/restricted/player/login.aspx",
                CompanyKey = "DUMMY-COMPANY-KEY"
            }
        }
    };

    public async Task Run()
    {
        while (true)
        {
            try
            {
                await ShowMenuAndExecute();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress Enter to continue or type 'exit' to quit.");
            var input = Console.ReadLine();
            if (input?.ToLower() == "exit")
            {
                break;
            }
        }
    }

    private async Task ShowMenuAndExecute()
    {
        Console.WriteLine("=== YY Game Launcher ===\n");

        Console.WriteLine("Select mode:");
        Console.WriteLine("1. Manual Input");
        Console.WriteLine("2. Auto Test Launches by Game with All Currency");
        Console.WriteLine("3. Auto Test Launches by Currency with All Games");
        Console.Write("Enter choice (1-3): ");
        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await ManualInput();
                break;
            case "2":
                await AutoTestLaunchesByGameWithAllCurrency();
                break;
            case "3":
                await AutoTestLaunchesByCurrencyWithAllGames();
                break;
            default:
                Console.WriteLine("Invalid choice. Please enter a number between 1 and 3.");
                break;
        }
    }

    private async Task AutoTestLaunchesByCurrencyWithAllGames()
    {
        Console.Write("GPID: ");
        var gpidInput = Console.ReadLine() ?? string.Empty;
        Console.Write("Currency:");
        var currency = Console.ReadLine();
        Console.Write("Environment (demo/yy/yy2/yy4/yy5/yy7/dr_yy1/dr_yy2/dr_yy4/dr_yy5): ");
        var env = Console.ReadLine() ?? string.Empty;

        var gameInfos = await GetGameList(gpidInput, env);
        foreach( var game in gameInfos.SeamlessGameProviderGames){
            var launchRequest = new LaunchRequest
            {
                Username = string.Empty,
                Gpid = gpidInput,
                GameId = game.GameID.ToString(),
                Env = env,
                Team = "qa",
                Device = "desktop",
                Language = "en",
                Betcode = string.Empty,
                ShowRequestUrl = false
            };

            // Find an account with the specified currency
            var accountInfo = RequestData[env]["qa"].AvailableUsers
                .FirstOrDefault(a => a.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase))?? throw new Exception("No account found");

            launchRequest.Username = accountInfo.Username;

            if (!ValidateParameters(launchRequest))
            {
                Console.WriteLine("Invalid parameters. Please check your input.");
                continue;
            }

            await LaunchGame(launchRequest);
        }
    }

    private async Task<MiranaGamelist> GetGameList(string gpidInput, string env)
    {
        var miranaHelper = new MiranaApiHelper();
        var gameList = await miranaHelper.FetchGameListAsync(gpidInput, env);
        return gameList;
    }

    private async Task AutoTestLaunchesByGameWithAllCurrency()
    {
        throw new NotImplementedException();
    }

    private async Task ManualInput()
    {
        var launchRequest = GetLaunchParameters();

        if (!ValidateParameters(launchRequest))
        {
            Console.WriteLine("Invalid parameters. Please check your input.");
            throw new Exception("Invalid parameters");
        }

        await LaunchGame(launchRequest);
    }

    private LaunchRequest GetLaunchParameters()
    {
        Console.Write("Username: ");
        var username = Console.ReadLine() ?? string.Empty;

        Console.Write("GPID: ");
        var gpidInput = Console.ReadLine() ?? string.Empty;

        Console.Write("Game ID: ");
        var gameIdInput = Console.ReadLine() ?? string.Empty;

        Console.Write("Environment (demo/yy/yy2/yy4/yy5/yy7/dr_yy1/dr_yy2/dr_yy4/dr_yy5): ");
        var env = Console.ReadLine() ?? string.Empty;

        Console.Write("Team (qa/aegina): ");
        var team = Console.ReadLine() ?? string.Empty;

        Console.Write("Device (default: desktop): ");
        var device = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(device)) device = "desktop";

        Console.Write("Language (default: en): ");
        var language = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(language)) language = "en";

        Console.Write("Betcode (optional, press Enter to skip): ");
        var betcode = Console.ReadLine() ?? string.Empty;

        Console.Write("Show request URL? (y/n, default: n): ");
        var showUrl = Console.ReadLine()?.ToLower() == "y";

        return new LaunchRequest
        {
            Username = username,
            Gpid = gpidInput,
            GameId = gameIdInput,
            Env = env,
            Team = team,
            Device = device,
            Language = language,
            Betcode = betcode,
            ShowRequestUrl = showUrl
        };
    }

    private bool ValidateParameters(LaunchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            Console.WriteLine("Error: Username is required.");
            return false;
        }

        if (!int.TryParse(request.Gpid, out _))
        {
            Console.WriteLine("Error: GPID must be a number.");
            return false;
        }

        if (!int.TryParse(request.GameId, out _))
        {
            Console.WriteLine("Error: Game ID must be a number.");
            return false;
        }

        if (!RequestData.ContainsKey(request.Env))
        {
            Console.WriteLine($"Error: Invalid environment '{request.Env}'.");
            return false;
        }

        if (!RequestData[request.Env].ContainsKey(request.Team))
        {
            Console.WriteLine($"Error: Invalid team '{request.Team}' for environment '{request.Env}'.");
            return false;
        }

        return true;
    }

    private async Task LaunchGame(LaunchRequest launchRequest)
    {
        var config = RequestData[launchRequest.Env][launchRequest.Team];

        var loginRequest = new LoginRequest
        {
            Username = launchRequest.Username,
            Portfolio = "SeamlessGame",
            IsWapSports = false,
            KYSportsbook = false,
            CompanyKey = config.CompanyKey
        };

        Console.WriteLine($"\nConnecting to: {config.Url}");
        Console.WriteLine($"Request: {JsonSerializer.Serialize(loginRequest, new JsonSerializerOptions { WriteIndented = true })}\n");

        try
        {
            using var httpClient = new HttpClient();
            var jsonContent = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(config.Url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (loginResponse?.Error?.Id == 0)
            {
                var gameUrl = BuildGameUrl(loginResponse.Url, launchRequest);

                if (launchRequest.ShowRequestUrl)
                {
                    Console.WriteLine($"\n=== Game Launch URL ===");
                    Console.WriteLine(gameUrl);
                    Console.Write("\nDo you want to open this URL in browser? (y/n): ");
                    var confirm = Console.ReadLine()?.ToLower();
                    if (confirm != "y")
                    {
                        Console.WriteLine("Launch cancelled by user.");
                        return;
                    }
                }

                OpenUrlInBrowser(gameUrl);
                Console.WriteLine("\nGame launched successfully!");
            }
            else
            {
                Console.WriteLine($"Error: {loginResponse?.Error?.Msg ?? "Unknown error"}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request error: {ex.Message}");
        }
    }

    private string BuildGameUrl(string baseUrl, LaunchRequest request)
    {
        var url = $"https:{baseUrl}&gpid={request.Gpid}&gameid={request.GameId}&device={request.Device}&lang={request.Language}";

        if (!string.IsNullOrWhiteSpace(request.Betcode))
        {
            url += $"&betcode={request.Betcode}";
        }

        return url;
    }

    private void OpenUrlInBrowser(string url)
    {
        try
        {
            // For macOS
            System.Diagnostics.Process.Start("open", url);
        }
        catch
        {
            // Fallback - just display the URL
            Console.WriteLine($"\nPlease open this URL in your browser:");
            Console.WriteLine(url);
        }
    }

    private class LaunchRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Gpid { get; set; } = string.Empty;
        public string GameId { get; set; } = string.Empty;
        public string Env { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Betcode { get; set; } = string.Empty;
        public bool ShowRequestUrl { get; set; }
    }

    private class EnvironmentConfig
    {
        public string Url { get; set; } = string.Empty;
        public string CompanyKey { get; set; } = string.Empty;
        public List<AccountInfo> AvailableUsers { get; set; } = new();
    }

    private class AccountInfo
    {
        public string Username { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
    }

    private class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Portfolio { get; set; } = string.Empty;
        public bool IsWapSports { get; set; }
        public bool KYSportsbook { get; set; }
        public string CompanyKey { get; set; } = string.Empty;
    }

    private class LoginResponse
    {
        public string Url { get; set; } = string.Empty;
        public ErrorInfo Error { get; set; } = new();
    }

    private class ErrorInfo
    {
        public int Id { get; set; }
        public string Msg { get; set; } = string.Empty;
    }
}
