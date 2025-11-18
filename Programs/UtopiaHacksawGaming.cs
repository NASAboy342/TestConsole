using System;
using Newtonsoft.Json;

namespace TestConsole.Programs;

public class UtopiaHacksawGaming
{
    private readonly List<string> providerSupportedCurrency = new List<string> { "AED", "AFN", "ALL", "AMD", "AOA", "ARS", "AZN", "BAM", "BDT", "BGN", "BND", "BOB", "BRL", "BWP", "BYN", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CZK", "DKK", "DOP", "DZD", "EGP", "ETB", "EUR", "GBP", "GEL", "GHS", "GNF", "GTQ", "HKD", "HNL", "HTG", "HUF", "IDO", "IDR", "ILS", "INR", "IQD", "ISK", "JOD", "JPY", "KES", "KGS", "KHR", "KRW", "KWD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "MAD", "MDL", "MKD", "MNT", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "PEN", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SEK", "THB", "TJS", "TMT", "TND", "TRY", "TTD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VES", "VND", "VNO", "XAF", "XOF", "ZAR" };
    private readonly List<AccountInfo> accountInfosOnDemo = new()
    {
        new AccountInfo{ Username = "gamesagtNPR", WebId = 2033, CustomerId = 589538,   Currency = "NPR"},
        new AccountInfo{ Username = "Sch1038LoginAgentCOP", WebId = 2033, CustomerId = 737438,  Currency = "COP"},
        new AccountInfo{ Username = "qaagentSCR", WebId = 2033, CustomerId = 840143,    Currency = "SCR"},
        new AccountInfo{ Username = "qaagentFKP", WebId = 2033, CustomerId = 840149,    Currency = "FKP"},
        new AccountInfo{ Username = "qaagentchf", WebId = 2033, CustomerId = 492182,    Currency = "CHF"},
        new AccountInfo{ Username = "qaagentGHS", WebId = 2033, CustomerId = 840097,    Currency = "GHS"},
        new AccountInfo{ Username = "qaagentPYG", WebId = 2033, CustomerId = 840136,    Currency = "PYG"},
        new AccountInfo{ Username = "testagentngn", WebId = 2033, CustomerId = 605054,  Currency = "NGN"},
        new AccountInfo{ Username = "QAagentnzd", WebId = 2033, CustomerId = 492222,    Currency = "NZD"},
        new AccountInfo{ Username = "qaagentBWP", WebId = 2033, CustomerId = 840082,    Currency = "BWP"},
        new AccountInfo{ Username = "qaagentGMD", WebId = 2033, CustomerId = 840098,    Currency = "GMD"},
        new AccountInfo{ Username = "qaagentTZS", WebId = 2033, CustomerId = 840163,    Currency = "TZS"},
        new AccountInfo{ Username = "qaAgentKRW", WebId = 2033, CustomerId = 490879,    Currency = "KRW"},
        new AccountInfo{ Username = "qaagentmmk", WebId = 2033, CustomerId = 490856,    Currency = "MMK"},
        new AccountInfo{ Username = "qaagentBAM", WebId = 2033, CustomerId = 840073,    Currency = "BAM"},
        new AccountInfo{ Username = "qaagentMRU", WebId = 2033, CustomerId = 840125,    Currency = "MRU"},
        new AccountInfo{ Username = "qaagentCUP", WebId = 2033, CustomerId = 840087,    Currency = "CUP"},
        new AccountInfo{ Username = "qaAgentHKD", WebId = 2033, CustomerId = 490859,    Currency = "HKD"},
        new AccountInfo{ Username = "qaagentBBD", WebId = 2033, CustomerId = 840074,    Currency = "BBD"},
        new AccountInfo{ Username = "qaagentKES", WebId = 2033, CustomerId = 840111,    Currency = "KES"},
        new AccountInfo{ Username = "qaagentNAD", WebId = 2033, CustomerId = 840130,    Currency = "NAD"},
        new AccountInfo{ Username = "qaagentAZN", WebId = 2033, CustomerId = 840072,    Currency = "AZN"},
        new AccountInfo{ Username = "qaagentBIF", WebId = 2033, CustomerId = 840077,    Currency = "BIF"},
        new AccountInfo{ Username = "qaagentSRD", WebId = 2033, CustomerId = 840147,    Currency = "SRD"},
        new AccountInfo{ Username = "qaagentRSD", WebId = 2033, CustomerId = 840139,    Currency = "RSD"},
        new AccountInfo{ Username = "SparkAgentPHP", WebId = 2033, CustomerId = 509883, Currency = "PHP"},
        new AccountInfo{ Username = "qaagentbdt3", WebId = 2033, CustomerId = 492396,   Currency = "BDT"},
        new AccountInfo{ Username = "qaagentQAR", WebId = 2033, CustomerId = 840137,    Currency = "QAR"},
        new AccountInfo{ Username = "qaagentJMD", WebId = 2033, CustomerId = 840109,    Currency = "JMD"},
        new AccountInfo{ Username = "qaagentAFN", WebId = 2033, CustomerId = 840065,    Currency = "AFN"},
        new AccountInfo{ Username = "qaagentHNL", WebId = 2033, CustomerId = 840102,    Currency = "HNL"},
        new AccountInfo{ Username = "qaagentERN", WebId = 2033, CustomerId = 840148,    Currency = "ERN"},
        new AccountInfo{ Username = "qaagentUZS", WebId = 2033, CustomerId = 840167,    Currency = "UZS"},
        new AccountInfo{ Username = "qaagentMWK", WebId = 2033, CustomerId = 840128,    Currency = "MWK"},
        new AccountInfo{ Username = "gamesagtMYK", WebId = 2033, CustomerId = 587407,   Currency = "MYK"},
        new AccountInfo{ Username = "qaagentSAR", WebId = 2033, CustomerId = 840141,    Currency = "SAR"},
        new AccountInfo{ Username = "qaagentSYP", WebId = 2033, CustomerId = 840154,    Currency = "SYP"},
        new AccountInfo{ Username = "qaagentALL", WebId = 2033, CustomerId = 840066,    Currency = "ALL"},
        new AccountInfo{ Username = "QAagentsek", WebId = 2033, CustomerId = 492225,    Currency = "SEK"},
        new AccountInfo{ Username = "qaagentKGS", WebId = 2033, CustomerId = 840112,    Currency = "KGS"},
        new AccountInfo{ Username = "qaagentmxn", WebId = 2033, CustomerId = 521861,    Currency = "MXN"},
        new AccountInfo{ Username = "qaagentrub", WebId = 2033, CustomerId = 510979,    Currency = "RUB"},
        new AccountInfo{ Username = "qaagentLYD", WebId = 2033, CustomerId = 840120,    Currency = "LYD"},
        new AccountInfo{ Username = "qaagentGIP", WebId = 2033, CustomerId = 840150,    Currency = "GIP"},
        new AccountInfo{ Username = "KHAgnetUSD", WebId = 2033, CustomerId = 478769,    Currency = "CNY"},
        new AccountInfo{ Username = "qaagentCRC", WebId = 2033, CustomerId = 840086,    Currency = "CRC"},
        new AccountInfo{ Username = "qaagentjpy1", WebId = 2033, CustomerId = 490868,   Currency = "JPY"},
        new AccountInfo{ Username = "qaagentJOD", WebId = 2033, CustomerId = 840110,    Currency = "JOD"},
        new AccountInfo{ Username = "qaagentNIO", WebId = 2033, CustomerId = 840131,    Currency = "NIO"},
        new AccountInfo{ Username = "KhAgentTHB", WebId = 2033, CustomerId = 480228,    Currency = "THB"},
        new AccountInfo{ Username = "QaAgentEUR", WebId = 2033, CustomerId = 490838,    Currency = "EUR"},
        new AccountInfo{ Username = "qaagentANG", WebId = 2033, CustomerId = 840068,    Currency = "ANG"},
        new AccountInfo{ Username = "qaagentKWD", WebId = 2033, CustomerId = 840114,    Currency = "KWD"},
        new AccountInfo{ Username = "qaagentLRD", WebId = 2033, CustomerId = 840118,    Currency = "LRD"},
        new AccountInfo{ Username = "qaagentAWG", WebId = 2033, CustomerId = 840071,    Currency = "AWG"},
        new AccountInfo{ Username = "qaagentVUV", WebId = 2033, CustomerId = 840155,    Currency = "VUV"},
        new AccountInfo{ Username = "qaagentBRL01", WebId = 2033, CustomerId = 518627,  Currency = "BRL"},
        new AccountInfo{ Username = "qaagentCDF", WebId = 2033, CustomerId = 840085,    Currency = "CDF"},
        new AccountInfo{ Username = "qaagentMGA", WebId = 2033, CustomerId = 840122,    Currency = "MGA"},
        new AccountInfo{ Username = "qaagentLBP", WebId = 2033, CustomerId = 840117,    Currency = "LBP"},
        new AccountInfo{ Username = "qaagentXCD", WebId = 2033, CustomerId = 840169,    Currency = "XCD"},
        new AccountInfo{ Username = "QaAgentMAD", WebId = 2033, CustomerId = 1553829,   Currency = "MAD"},
        new AccountInfo{ Username = "qaAgentMYR", WebId = 2033, CustomerId = 490881,    Currency = "MYR"},
        new AccountInfo{ Username = "qaagentDKK", WebId = 2033, CustomerId = 840091,    Currency = "DKK"},
        new AccountInfo{ Username = "qaagentLSL", WebId = 2033, CustomerId = 840119,    Currency = "LSL"},
        new AccountInfo{ Username = "qaAgentGBP", WebId = 2033, CustomerId = 490852,    Currency = "GBP"},
        new AccountInfo{ Username = "qaagentAOA", WebId = 2033, CustomerId = 840069,    Currency = "AOA"},
        new AccountInfo{ Username = "qaagentDOP", WebId = 2033, CustomerId = 840092,    Currency = "DOP"},
        new AccountInfo{ Username = "qaagentMOP", WebId = 2033, CustomerId = 840124,    Currency = "MOP"},
        new AccountInfo{ Username = "QaAgentDZD", WebId = 2033, CustomerId = 1553826,   Currency = "DZD"},
        new AccountInfo{ Username = "ttomAgentGDA", WebId = 2033, CustomerId = 481231,  Currency = "GDA"},
        new AccountInfo{ Username = "qaagentBHD", WebId = 2033, CustomerId = 840076,    Currency = "BHD"},
        new AccountInfo{ Username = "qaagentbnd", WebId = 2033, CustomerId = 494445,    Currency = "BND"},
        new AccountInfo{ Username = "qaagentTMT", WebId = 2033, CustomerId = 840159,    Currency = "TMT"},
        new AccountInfo{ Username = "qaagentSZL", WebId = 2033, CustomerId = 840157,    Currency = "SZL"},
        new AccountInfo{ Username = "qaagentaud", WebId = 2033, CustomerId = 490835,    Currency = "AUD"},
        new AccountInfo{ Username = "qaagentOMR", WebId = 2033, CustomerId = 840132,    Currency = "OMR"},
        new AccountInfo{ Username = "qaagentMZN", WebId = 2033, CustomerId = 840129,    Currency = "MZN"},
        new AccountInfo{ Username = "qaagentPAB", WebId = 2033, CustomerId = 840133,    Currency = "PAB"},
        new AccountInfo{ Username = "sstestLKR", WebId = 2033, CustomerId = 610189, Currency = "LKR"},
        new AccountInfo{ Username = "qaagentRWF", WebId = 2033, CustomerId = 840140,    Currency = "RWF"},
        new AccountInfo{ Username = "qaagentAED", WebId = 2033, CustomerId = 521753,    Currency = "AED"},
        new AccountInfo{ Username = "qaagentEGP", WebId = 2033, CustomerId = 840093,    Currency = "EGP"},
        new AccountInfo{ Username = "qaagentTJS", WebId = 2033, CustomerId = 840158,    Currency = "TJS"},
        new AccountInfo{ Username = "LeoAgent", WebId = 2033, CustomerId = 4159,    Currency = "IDR"},
        new AccountInfo{ Username = "qaagentCZK", WebId = 2033, CustomerId = 840089,    Currency = "CZK"},
        new AccountInfo{ Username = "qaagentSDG", WebId = 2033, CustomerId = 840144,    Currency = "SDG"},
        new AccountInfo{ Username = "QaAgentUCC", WebId = 2033, CustomerId = 718444,    Currency = "UCC"},
        new AccountInfo{ Username = "qaagentARS", WebId = 2033, CustomerId = 840070,    Currency = "ARS"},
        new AccountInfo{ Username = "qaagentBMD", WebId = 2033, CustomerId = 840078,    Currency = "BMD"},
        new AccountInfo{ Username = "sstestTRY", WebId = 2033, CustomerId = 610185, Currency = "TRY"},
        new AccountInfo{ Username = "qaagentlak", WebId = 2033, CustomerId = 492216,    Currency = "LAK"},
        new AccountInfo{ Username = "qaagentDJF", WebId = 2033, CustomerId = 840090,    Currency = "DJF"},
        new AccountInfo{ Username = "qaagentKMF", WebId = 2033, CustomerId = 840113,    Currency = "KMF"},
        new AccountInfo{ Username = "qaagentYER", WebId = 2033, CustomerId = 840172,    Currency = "YER"},
        new AccountInfo{ Username = "sstestPKR", WebId = 2033, CustomerId = 610187, Currency = "PKR"},
        new AccountInfo{ Username = "qaagentCVE", WebId = 2033, CustomerId = 840088,    Currency = "CVE"},
        new AccountInfo{ Username = "qaagentGEL", WebId = 2033, CustomerId = 840096,    Currency = "GEL"},
        new AccountInfo{ Username = "Sch1038LoginAgentVES", WebId = 2033, CustomerId = 737480,  Currency = "VES"},
        new AccountInfo{ Username = "qaagentSLL", WebId = 2033, CustomerId = 840145,    Currency = "SLL"},
        new AccountInfo{ Username = "QaAgentPEN", WebId = 2033, CustomerId = 718442,    Currency = "PEN"},
        new AccountInfo{ Username = "APITTestAgentIDO01", WebId = 2033, CustomerId = 481195,    Currency = "IDO"},
        new AccountInfo{ Username = "qaagentAMD", WebId = 2033, CustomerId = 840067,    Currency = "AMD"},
        new AccountInfo{ Username = "qaagentXAF", WebId = 2033, CustomerId = 840168,    Currency = "XAF"},
        new AccountInfo{ Username = "AgentMnt", WebId = 2033, CustomerId = 734101,  Currency = "MNT"},
        new AccountInfo{ Username = "qaagentGYD", WebId = 2033, CustomerId = 840101,    Currency = "GYD"},
        new AccountInfo{ Username = "qaagentBTN", WebId = 2033, CustomerId = 840081,    Currency = "BTN"},
        new AccountInfo{ Username = "qaagentKZT", WebId = 2033, CustomerId = 840116,    Currency = "KZT"},
        new AccountInfo{ Username = "qaagentILS", WebId = 2033, CustomerId = 840105,    Currency = "ILS"},
        new AccountInfo{ Username = "qaagentSBD", WebId = 2033, CustomerId = 840142,    Currency = "SBD"},
        new AccountInfo{ Username = "qaagentcad", WebId = 2033, CustomerId = 492174,    Currency = "CAD"},
        new AccountInfo{ Username = "qaagentKYD", WebId = 2033, CustomerId = 840115,    Currency = "KYD"},
        new AccountInfo{ Username = "qaagentIQD", WebId = 2033, CustomerId = 840106,    Currency = "IQD"},
        new AccountInfo{ Username = "qaagentETB", WebId = 2033, CustomerId = 840094,    Currency = "ETB"},
        new AccountInfo{ Username = "qaagentXPF", WebId = 2033, CustomerId = 840171,    Currency = "XPF"},
        new AccountInfo{ Username = "qaagentUAH", WebId = 2033, CustomerId = 840164,    Currency = "UAH"},
        new AccountInfo{ Username = "qaagentMUR", WebId = 2033, CustomerId = 840126,    Currency = "MUR"},
        new AccountInfo{ Username = "sc8282", WebId = 2033, CustomerId = 5951,  Currency = "TMP"},
        new AccountInfo{ Username = "qaagentUYU", WebId = 2033, CustomerId = 840166,    Currency = "UYU"},
        new AccountInfo{ Username = "qaagentBSD", WebId = 2033, CustomerId = 840080,    Currency = "BSD"},
        new AccountInfo{ Username = "qaagentKPW", WebId = 2033, CustomerId = 840151,    Currency = "KPW"},
        new AccountInfo{ Username = "qaagentFJD", WebId = 2033, CustomerId = 840095,    Currency = "FJD"},
        new AccountInfo{ Username = "qaagentXOF", WebId = 2033, CustomerId = 840170,    Currency = "XOF"},
        new AccountInfo{ Username = "qaagentTTD", WebId = 2033, CustomerId = 840162,    Currency = "TTD"},
        new AccountInfo{ Username = "qaagentBYN", WebId = 2033, CustomerId = 840083,    Currency = "BYN"},
        new AccountInfo{ Username = "qaagentWST", WebId = 2033, CustomerId = 840156,    Currency = "WST"},
        new AccountInfo{ Username = "qaagentGTQ", WebId = 2033, CustomerId = 840100,    Currency = "GTQ"},
        new AccountInfo{ Username = "qaagentNOK01", WebId = 2033, CustomerId = 518634,  Currency = "NOK"},
        new AccountInfo{ Username = "qaagentvnd", WebId = 2033, CustomerId = 490844,    Currency = "VND"},
        new AccountInfo{ Username = "qaagentPGK", WebId = 2033, CustomerId = 840134,    Currency = "PGK"},
        new AccountInfo{ Username = "qaagentBGN", WebId = 2033, CustomerId = 840075,    Currency = "BGN"},
        new AccountInfo{ Username = "qaagentMVR", WebId = 2033, CustomerId = 840127,    Currency = "MVR"},
        new AccountInfo{ Username = "qaagentBOB", WebId = 2033, CustomerId = 840079,    Currency = "BOB"},
        new AccountInfo{ Username = "qaagentHTG", WebId = 2033, CustomerId = 840103,    Currency = "HTG"},
        new AccountInfo{ Username = "qaagentIRR", WebId = 2033, CustomerId = 840107,    Currency = "IRR"},
        new AccountInfo{ Username = "qaagentTND", WebId = 2033, CustomerId = 840160,    Currency = "TND"},
        new AccountInfo{ Username = "qaagentBZD", WebId = 2033, CustomerId = 840084,    Currency = "BZD"},
        new AccountInfo{ Username = "qaagentMDL", WebId = 2033, CustomerId = 840121,    Currency = "MDL"},
        new AccountInfo{ Username = "qaagentZAR01", WebId = 2033, CustomerId = 518639,  Currency = "ZAR"},
        new AccountInfo{ Username = "QaAgentCLP", WebId = 2033, CustomerId = 718439,    Currency = "CLP"},
        new AccountInfo{ Username = "qaagentSOS", WebId = 2033, CustomerId = 840146,    Currency = "SOS"},
        new AccountInfo{ Username = "qaagentUGX", WebId = 2033, CustomerId = 840165,    Currency = "UGX"},
        new AccountInfo{ Username = "APITTestAgentVNO01", WebId = 2033, CustomerId = 481197,    Currency = "VNO"},
        new AccountInfo{ Username = "qaagentSSP", WebId = 2033, CustomerId = 840152,    Currency = "SSP"},
        new AccountInfo{ Username = "qaagentHUF", WebId = 2033, CustomerId = 840104,    Currency = "HUF"},
        new AccountInfo{ Username = "qaagentZMW", WebId = 2033, CustomerId = 840173,    Currency = "ZMW"},
        new AccountInfo{ Username = "qaagentMKD", WebId = 2033, CustomerId = 840123,    Currency = "MKD"},
        new AccountInfo{ Username = "qaagentinr", WebId = 2033, CustomerId = 490875,    Currency = "INR"},
        new AccountInfo{ Username = "qaagentSTN", WebId = 2033, CustomerId = 840153,    Currency = "STN"},
        new AccountInfo{ Username = "jjtestAgentKHR", WebId = 2033, CustomerId = 591109,    Currency = "KHR"},
        new AccountInfo{ Username = "qaagentGNF", WebId = 2033, CustomerId = 840099,    Currency = "GNF"},
        new AccountInfo{ Username = "TestAgentUSD", WebId = 2033, CustomerId = 469326,  Currency = "USD"},
        new AccountInfo{ Username = "qaagentISK", WebId = 2033, CustomerId = 840108,    Currency = "ISK"},
        new AccountInfo{ Username = "qaagentPLN", WebId = 2033, CustomerId = 840135,    Currency = "PLN"},
        new AccountInfo{ Username = "qaagentRON", WebId = 2033, CustomerId = 840138,    Currency = "RON"},
        new AccountInfo{ Username = "qaagentTOP", WebId = 2033, CustomerId = 840161,    Currency = "TOP"}
    };
    internal async Task TestAllCurrencyByGameDemo(int gameId)
    {
        foreach (var account in accountInfosOnDemo)
        {
            if(!providerSupportedCurrency.Contains(account.Currency))
            {
                continue;
            }
            var username = $"{account.WebId}SBONS{account.CustomerId}";
            var customerId = account.CustomerId;
            var accountId = $"{account.WebId}yy_{account.Username}";
            var currency = account.Currency;

            var loginResponse = await CallLogin(username, customerId, accountId, gameId, currency);
            if (loginResponse.IsSuccess)
            {
                Console.WriteLine($"Login Success! With Currency {currency} Launch URL: {loginResponse.Result}");
            }
            else
            {
                Console.WriteLine($"Login Failed! ErrorCode: {loginResponse.ErrorCode}, ErrorMessage: {loginResponse.ErrorMessage}");
            }
        }
        Console.WriteLine("All done!");
    }

    private static async Task<LoginResponse> CallLogin(string username, int customerId, string accountId, int gameId, string currency)
    {
        var loginRequest = new LoginRequest()
        {
            User = username,
            ProcessLoginModel = new ProcessLoginModel
            {
                GameId = gameId,
                GpId = 1096,
                Lang = 1,
                IsPlayForReal = true,
                FpId = 0,
                Device = "d",
                IsApp = false,
                IsLoginToSpecificGame = false,
                WebId = 0,
                GameCode = "",
                GameHall = "",
                HomeUrl = "",
                BetCode = "",
                Ip = "136.228.131.134",
                MarsDomain = "lmd-uat.gaolitsai.com",
                UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0"
            },
            PlayerLoginInfoModel = new PlayerLoginInfoModel
            {
                CustomerId = customerId,
                IsTest = 0,
                PlayerInfo = new PlayerInfo
                {
                    CustomerID = customerId,
                    AccountID = accountId,
                    Credit = 0,
                    Outstanding = 0.0m,
                    CashBalance = 0.0m,
                    Reward = 0,
                    Currency = currency,
                    Status = 0,
                    DisplayName = null,
                    Ladder = 0,
                    Experience = 0,
                    LastLoginIP = "Mirana Ip",
                    LastLoginTime = null,
                    PasswordExpiryDate = null,
                    CanChangeDisplayName = false,
                    CanChangeLoginName = false,
                    FirstTimeSignOn = false,
                    ProductAvailable = null,
                    TableLimit = 0,
                    FpID = 0,
                    OddsStyle = 0,
                    OddsMode = null,
                    WebId = 2033
                }
            }
        };
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, "https://capi-uat-hacksawgaming.csmc-api.com/Provider/Login");
        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        var loginResponsestring = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(loginResponsestring);
        return loginResponse;
    }
}

public class LoginRequest
{
    [JsonProperty("User")]
    public string User { get; set; } = string.Empty;

    [JsonProperty("ProcessLoginModel")]
    public ProcessLoginModel ProcessLoginModel { get; set; } = new();

    [JsonProperty("PlayerLoginInfoModel")]
    public PlayerLoginInfoModel PlayerLoginInfoModel { get; set; } = new();
}

public class ProcessLoginModel
{
    [JsonProperty("GameId")]
    public int GameId { get; set; }

    [JsonProperty("GpId")]
    public int GpId { get; set; }

    [JsonProperty("Lang")]
    public int Lang { get; set; }

    [JsonProperty("IsPlayForReal")]
    public bool IsPlayForReal { get; set; }

    [JsonProperty("FpId")]
    public int FpId { get; set; }

    [JsonProperty("Device")]
    public string Device { get; set; } = string.Empty;

    [JsonProperty("IsApp")]
    public bool IsApp { get; set; }

    [JsonProperty("IsLoginToSpecificGame")]
    public bool IsLoginToSpecificGame { get; set; }

    [JsonProperty("WebId")]
    public int WebId { get; set; }

    [JsonProperty("GameCode")]
    public string GameCode { get; set; } = string.Empty;

    [JsonProperty("GameHall")]
    public string GameHall { get; set; } = string.Empty;

    [JsonProperty("HomeUrl")]
    public string HomeUrl { get; set; } = string.Empty;

    [JsonProperty("BetCode")]
    public string BetCode { get; set; } = string.Empty;

    [JsonProperty("Ip")]
    public string Ip { get; set; } = string.Empty;

    [JsonProperty("MarsDomain")]
    public string MarsDomain { get; set; } = string.Empty;

    [JsonProperty("UserAgent")]
    public string UserAgent { get; set; } = string.Empty;
}

public class PlayerLoginInfoModel
{
    [JsonProperty("CustomerId")]
    public int CustomerId { get; set; }

    [JsonProperty("IsTest")]
    public int IsTest { get; set; }

    [JsonProperty("PlayerInfo")]
    public PlayerInfo PlayerInfo { get; set; } = new();
}

public class PlayerInfo
{
    [JsonProperty("CustomerID")]
    public int CustomerID { get; set; }

    [JsonProperty("AccountID")]
    public string AccountID { get; set; } = string.Empty;

    [JsonProperty("Credit")]
    public int Credit { get; set; }

    [JsonProperty("Outstanding")]
    public decimal Outstanding { get; set; }

    [JsonProperty("CashBalance")]
    public decimal CashBalance { get; set; }

    [JsonProperty("Reward")]
    public int Reward { get; set; }

    [JsonProperty("Currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonProperty("Status")]
    public int Status { get; set; }

    [JsonProperty("DisplayName")]
    public string? DisplayName { get; set; }

    [JsonProperty("Ladder")]
    public int Ladder { get; set; }

    [JsonProperty("Experience")]
    public int Experience { get; set; }

    [JsonProperty("LastLoginIP")]
    public string LastLoginIP { get; set; } = string.Empty;

    [JsonProperty("LastLoginTime")]
    public DateTime? LastLoginTime { get; set; }

    [JsonProperty("PasswordExpiryDate")]
    public DateTime? PasswordExpiryDate { get; set; }

    [JsonProperty("CanChangeDisplayName")]
    public bool CanChangeDisplayName { get; set; }

    [JsonProperty("CanChangeLoginName")]
    public bool CanChangeLoginName { get; set; }

    [JsonProperty("FirstTimeSignOn")]
    public bool FirstTimeSignOn { get; set; }

    [JsonProperty("ProductAvailable")]
    public string? ProductAvailable { get; set; }

    [JsonProperty("TableLimit")]
    public int TableLimit { get; set; }

    [JsonProperty("FpID")]
    public int FpID { get; set; }

    [JsonProperty("OddsStyle")]
    public int OddsStyle { get; set; }

    [JsonProperty("OddsMode")]
    public string? OddsMode { get; set; }

    [JsonProperty("WebId")]
    public int WebId { get; set; }
}

public class LoginResponse
{
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;

    public bool IsSuccess => ErrorCode == 0;
}

public class AccountInfo
{
    public string Username { get; set; } = string.Empty;
    public int WebId { get; set; } = 0;
    public int CustomerId { get; set; } = 0;
    public string Currency { get; set; } = string.Empty;
}