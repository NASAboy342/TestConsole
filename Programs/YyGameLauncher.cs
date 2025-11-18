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
                AvailableUsers = new List<AccountInfo>{
                    new AccountInfo { Username = "testAgentnull", Currency =    "NPR" },
                    new AccountInfo { Username = "testagentCOP", Currency = "COP" },
                    new AccountInfo { Username = "testplayerSCR", Currency =    "SCR" },
                    new AccountInfo { Username = "qaagentFKP", Currency =   "FKP" },
                    new AccountInfo { Username = "qaagentchf", Currency =   "CHF" },
                    new AccountInfo { Username = "testplayerGHS", Currency =    "GHS" },
                    new AccountInfo { Username = "testplayerPYG", Currency =    "PYG" },
                    new AccountInfo { Username = "xb00_704", Currency = "NGN" },
                    new AccountInfo { Username = "QAagentnzd", Currency =   "NZD" },
                    new AccountInfo { Username = "testAgentBWP", Currency = "BWP" },
                    new AccountInfo { Username = "testplayerGMD", Currency =    "GMD" },
                    new AccountInfo { Username = "testplayerTZS", Currency =    "TZS" },
                    new AccountInfo { Username = "7604784KRW2fd2", Currency =   "KRW" },
                    new AccountInfo { Username = "AgentMMK", Currency = "MMK" },
                    new AccountInfo { Username = "testAgentBAM", Currency = "BAM" },
                    new AccountInfo { Username = "testplayerMRU", Currency =    "MRU" },
                    new AccountInfo { Username = "testAgentCUP", Currency = "CUP" },
                    new AccountInfo { Username = "chokwong7", Currency =    "HKD" },
                    new AccountInfo { Username = "testAgentBBD", Currency = "BBD" },
                    new AccountInfo { Username = "testplayerKES", Currency =    "KES" },
                    new AccountInfo { Username = "testplayerNAD", Currency =    "NAD" },
                    new AccountInfo { Username = "testAgentAZN", Currency = "AZN" },
                    new AccountInfo { Username = "testAgentBIF", Currency = "BIF" },
                    new AccountInfo { Username = "testplayerSRD", Currency =    "SRD" },
                    new AccountInfo { Username = "testplayerRSD", Currency =    "RSD" },
                    new AccountInfo { Username = "AgenTeja03", Currency =   "PHP" },
                    new AccountInfo { Username = "qaagentbdt3", Currency =  "BDT" },
                    new AccountInfo { Username = "testplayerQAR", Currency =    "QAR" },
                    new AccountInfo { Username = "testplayerJMD", Currency =    "JMD" },
                    new AccountInfo { Username = "testAgentAFN", Currency = "AFN" },
                    new AccountInfo { Username = "testplayerHNL", Currency =    "HNL" },
                    new AccountInfo { Username = "AgentERN7787", Currency = "ERN" },
                    new AccountInfo { Username = "testplayerUZS", Currency =    "UZS" },
                    new AccountInfo { Username = "testplayerMWK", Currency =    "MWK" },
                    new AccountInfo { Username = "uecC5uSfOGEQLs_MYK", Currency =   "MYK" },
                    new AccountInfo { Username = "c2uatagSAR_", Currency =  "SAR" },
                    new AccountInfo { Username = "qaagentSYP", Currency =   "SYP" },
                    new AccountInfo { Username = "testAgentALL", Currency = "ALL" },
                    new AccountInfo { Username = "QAagentsek", Currency =   "SEK" },
                    new AccountInfo { Username = "testplayerKGS", Currency =    "KGS" },
                    new AccountInfo { Username = "qaagentmxn", Currency =   "MXN" },
                    new AccountInfo { Username = "qaagentrub", Currency =   "RUB" },
                    new AccountInfo { Username = "testplayerLYD", Currency =    "LYD" },
                    new AccountInfo { Username = "qaagentGIP", Currency =   "GIP" },
                    new AccountInfo { Username = "liang01TC3", Currency =   "CNY" },
                    new AccountInfo { Username = "testAgentCRC", Currency = "CRC" },
                    new AccountInfo { Username = "qaagentjpy1", Currency =  "JPY" },
                    new AccountInfo { Username = "testplayerJOD", Currency =    "JOD" },
                    new AccountInfo { Username = "testplayerNIO", Currency =    "NIO" },
                    new AccountInfo { Username = "sbo00565un04moss", Currency = "THB" },
                    new AccountInfo { Username = "QaAgentEUR", Currency =   "EUR" },
                    new AccountInfo { Username = "testAgentANG", Currency = "ANG" },
                    new AccountInfo { Username = "testplayerKWD", Currency =    "KWD" },
                    new AccountInfo { Username = "testplayerLRD", Currency =    "LRD" },
                    new AccountInfo { Username = "testAgentAWG", Currency = "AWG" },
                    new AccountInfo { Username = "qaagentVUV", Currency =   "VUV" },
                    new AccountInfo { Username = "qaagentBRL01", Currency = "BRL" },
                    new AccountInfo { Username = "testAgentCDF", Currency = "CDF" },
                    new AccountInfo { Username = "testplayerMGA", Currency =    "MGA" },
                    new AccountInfo { Username = "testplayerLBP", Currency =    "LBP" },
                    new AccountInfo { Username = "testplayerXCD", Currency =    "XCD" },
                    new AccountInfo { Username = "testAgentMAD", Currency = "MAD" },
                    new AccountInfo { Username = "IDRTEST", Currency =  "MYR" },
                    new AccountInfo { Username = "testplayerDKK", Currency =    "DKK" },
                    new AccountInfo { Username = "testplayerLSL", Currency =    "LSL" },
                    new AccountInfo { Username = "GBPdevelopmentnagagaming", Currency = "GBP" },
                    new AccountInfo { Username = "testAgentAOA", Currency = "AOA" },
                    new AccountInfo { Username = "testplayerDOP", Currency =    "DOP" },
                    new AccountInfo { Username = "testplayerMOP", Currency =    "MOP" },
                    new AccountInfo { Username = "testAgentDZD", Currency = "DZD" },
                    new AccountInfo { Username = "ttomAgentGDA", Currency = "GDA" },
                    new AccountInfo { Username = "testAgentBHD", Currency = "BHD" },
                    new AccountInfo { Username = "qaagentbnd", Currency =   "BND" },
                    new AccountInfo { Username = "testplayerTMT", Currency =    "TMT" },
                    new AccountInfo { Username = "testplayerSZL", Currency =    "SZL" },
                    new AccountInfo { Username = "jjtest", Currency =   "AUD" },
                    new AccountInfo { Username = "c2uatagOMR", Currency =   "OMR" },
                    new AccountInfo { Username = "testplayerMZN", Currency =    "MZN" },
                    new AccountInfo { Username = "testplayerPAB", Currency =    "PAB" },
                    new AccountInfo { Username = "testAgentLKR", Currency = "LKR" },
                    new AccountInfo { Username = "testplayerRWF", Currency =    "RWF" },
                    new AccountInfo { Username = "qaagentAED", Currency =   "AED" },
                    new AccountInfo { Username = "Agent_EGP", Currency =    "EGP" },
                    new AccountInfo { Username = "testplayerTJS", Currency =    "TJS" },
                    new AccountInfo { Username = "ibetIDR", Currency =  "IDR" },
                    new AccountInfo { Username = "testAgentCZK", Currency = "CZK" },
                    new AccountInfo { Username = "testplayerSDG", Currency =    "SDG" },
                    new AccountInfo { Username = "mikaeagentucc", Currency =    "UCC" },
                    new AccountInfo { Username = "testAgentARS", Currency = "ARS" },
                    new AccountInfo { Username = "testAgentBMD", Currency = "BMD" },
                    new AccountInfo { Username = "c2uatagTRY", Currency =   "TRY" },
                    new AccountInfo { Username = "btbetlak", Currency = "LAK" },
                    new AccountInfo { Username = "testAgentDJF", Currency = "DJF" },
                    new AccountInfo { Username = "testplayerKMF", Currency =    "KMF" },
                    new AccountInfo { Username = "testplayerYER", Currency =    "YER" },
                    new AccountInfo { Username = "AFBGG_PKR", Currency =    "PKR" },
                    new AccountInfo { Username = "testAgentCVE", Currency = "CVE" },
                    new AccountInfo { Username = "testplayerGEL", Currency =    "GEL" },
                    new AccountInfo { Username = "testagentVES", Currency = "VES" },
                    new AccountInfo { Username = "testplayerSLL", Currency =    "SLL" },
                    new AccountInfo { Username = "testagentpen", Currency = "PEN" },
                    new AccountInfo { Username = "APITTestAgentIDO01", Currency =   "IDO" },
                    new AccountInfo { Username = "testAgentAMD", Currency = "AMD" },
                    new AccountInfo { Username = "testplayerXAF", Currency =    "XAF" },
                    new AccountInfo { Username = "testAgentMNT", Currency = "MNT" },
                    new AccountInfo { Username = "testplayerGYD", Currency =    "GYD" },
                    new AccountInfo { Username = "testAgentBTN", Currency = "BTN" },
                    new AccountInfo { Username = "AFBGG_KZT01", Currency =  "KZT" },
                    new AccountInfo { Username = "testplayerILS", Currency =    "ILS" },
                    new AccountInfo { Username = "testplayerSBD", Currency =    "SBD" },
                    new AccountInfo { Username = "qaagentcad", Currency =   "CAD" },
                    new AccountInfo { Username = "testplayerKYD", Currency =    "KYD" },
                    new AccountInfo { Username = "testplayerIQD", Currency =    "IQD" },
                    new AccountInfo { Username = "testplayerETB", Currency =    "ETB" },
                    new AccountInfo { Username = "testplayerXPF", Currency =    "XPF" },
                    new AccountInfo { Username = "testplayerUAH", Currency =    "UAH" },
                    new AccountInfo { Username = "testplayerMUR", Currency =    "MUR" },
                    new AccountInfo { Username = "kurt09123578", Currency = "TMP" },
                    new AccountInfo { Username = "testplayerUYU", Currency =    "UYU" },
                    new AccountInfo { Username = "testAgentBSD", Currency = "BSD" },
                    new AccountInfo { Username = "AgentKPW7787", Currency = "KPW" },
                    new AccountInfo { Username = "testplayerFJD", Currency =    "FJD" },
                    new AccountInfo { Username = "testplayerXOF", Currency =    "XOF" },
                    new AccountInfo { Username = "testplayerTTD", Currency =    "TTD" },
                    new AccountInfo { Username = "testAgentBYN", Currency = "BYN" },
                    new AccountInfo { Username = "qaagentWST", Currency =   "WST" },
                    new AccountInfo { Username = "testplayerGTQ", Currency =    "GTQ" },
                    new AccountInfo { Username = "qaagentNOK01", Currency = "NOK" },
                    new AccountInfo { Username = "ibetVND", Currency =  "VND" },
                    new AccountInfo { Username = "testplayerPGK", Currency =    "PGK" },
                    new AccountInfo { Username = "testAgentBGN", Currency = "BGN" },
                    new AccountInfo { Username = "testplayerMVR", Currency =    "MVR" },
                    new AccountInfo { Username = "testAgentBOB", Currency = "BOB" },
                    new AccountInfo { Username = "testplayerHTG", Currency =    "HTG" },
                    new AccountInfo { Username = "testplayerIRR", Currency =    "IRR" },
                    new AccountInfo { Username = "testplayerTND", Currency =    "TND" },
                    new AccountInfo { Username = "testAgentBZD", Currency = "BZD" },
                    new AccountInfo { Username = "testplayerMDL", Currency =    "MDL" },
                    new AccountInfo { Username = "qaagentZAR01", Currency = "ZAR" },
                    new AccountInfo { Username = "QaAgentCLP", Currency =   "CLP" },
                    new AccountInfo { Username = "testplayerSOS", Currency =    "SOS" },
                    new AccountInfo { Username = "testplayerUGX", Currency =    "UGX" },
                    new AccountInfo { Username = "APITTestAgentVNO01", Currency =   "VNO" },
                    new AccountInfo { Username = "qaagentSSP", Currency =   "SSP" },
                    new AccountInfo { Username = "testplayerHUF", Currency =    "HUF" },
                    new AccountInfo { Username = "testplayerZMW", Currency =    "ZMW" },
                    new AccountInfo { Username = "testplayerMKD", Currency =    "MKD" },
                    new AccountInfo { Username = "qaagentinr", Currency =   "INR" },
                    new AccountInfo { Username = "qaagentSTN", Currency =   "STN" },
                    new AccountInfo { Username = "TestPlayer001", Currency =    "KHR" },
                    new AccountInfo { Username = "testplayerGNF", Currency =    "GNF" },
                    new AccountInfo { Username = "testusd001", Currency =   "USD" },
                    new AccountInfo { Username = "testplayerISK", Currency =    "ISK" },
                    new AccountInfo { Username = "testplayerPLN", Currency =    "PLN" },
                    new AccountInfo { Username = "testplayerRON", Currency =    "RON" },
                    new AccountInfo { Username = "testplayerTOP", Currency =    "TOP" }
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
