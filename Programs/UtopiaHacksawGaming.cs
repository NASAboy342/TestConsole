using System;
using Newtonsoft.Json;

namespace TestConsole.Programs;

public class UtopiaHacksawGaming
{
    private readonly List<string> providerSupportedCurrency = new List<string> { "AED", "AFN", "ALL", "AMD", "AOA", "ARS", "AZN", "BAM", "BDT", "BGN", "BND", "BOB", "BRL", "BWP", "BYN", "CAD", "CDF", "CHF", "CLP", "CNY", "COP", "CRC", "CZK", "DKK", "DOP", "DZD", "EGP", "ETB", "EUR", "GBP", "GEL", "GHS", "GNF", "GTQ", "HKD", "HNL", "HTG", "HUF", "IDO", "IDR", "ILS", "INR", "IQD", "ISK", "JOD", "JPY", "KES", "KGS", "KHR", "KRW", "KWD", "KZT", "LAK", "LBP", "LKR", "LRD", "LSL", "MAD", "MDL", "MKD", "MNT", "MWK", "MXN", "MYR", "MZN", "NAD", "NGN", "NIO", "NOK", "NPR", "NZD", "PEN", "PHP", "PKR", "PLN", "PYG", "QAR", "RON", "RSD", "RUB", "RWF", "SAR", "SEK", "THB", "TJS", "TMT", "TND", "TRY", "TTD", "TZS", "UAH", "UGX", "USD", "UYU", "UZS", "VES", "VND", "VNO", "XAF", "XOF", "ZAR" };
    private readonly List<AccountInfo> accountInfosOnDemo = new()
    {
        new AccountInfo{ Username = "qatestaed",	WebId =2033,	CustomerId = 521754,	Currency = "AED" },
        new AccountInfo{ Username = "qatestAFN",	WebId =2033,	CustomerId = 840175,	Currency = "AFN" },
        new AccountInfo{ Username = "qatestALL",	WebId =2033,	CustomerId = 840176,	Currency = "ALL" },
        new AccountInfo{ Username = "qatestAMD",	WebId =2033,	CustomerId = 840177,	Currency = "AMD" },
        new AccountInfo{ Username = "qatestANG",	WebId =2033,	CustomerId = 840178,	Currency = "ANG" },
        new AccountInfo{ Username = "qatestAOA",	WebId =2033,	CustomerId = 840179,	Currency = "AOA" },
        new AccountInfo{ Username = "qatestARS",	WebId =2033,	CustomerId = 840180,	Currency = "ARS" },
        new AccountInfo{ Username = "qatestaud",	WebId =2033,	CustomerId = 490836,	Currency = "AUD" },
        new AccountInfo{ Username = "qatestAWG",	WebId =2033,	CustomerId = 840181,	Currency = "AWG" },
        new AccountInfo{ Username = "qatestAZN",	WebId =2033,	CustomerId = 840182,	Currency = "AZN" },
        new AccountInfo{ Username = "qatestBAM",	WebId =2033,	CustomerId = 840183,	Currency = "BAM" },
        new AccountInfo{ Username = "qatestBBD",	WebId =2033,	CustomerId = 840184,	Currency = "BBD" },
        new AccountInfo{ Username = "CssgUatPlayerBDT",	WebId =2033,	CustomerId = 492378,	Currency = "BDT" },
        new AccountInfo{ Username = "qatestBGN",	WebId =2033,	CustomerId = 840185,	Currency = "BGN" },
        new AccountInfo{ Username = "qatestBHD",	WebId =2033,	CustomerId = 840186,	Currency = "BHD" },
        new AccountInfo{ Username = "qatestBIF",	WebId =2033,	CustomerId = 840187,	Currency = "BIF" },
        new AccountInfo{ Username = "qatestBMD",	WebId =2033,	CustomerId = 840188,	Currency = "BMD" },
        new AccountInfo{ Username = "CssgUatPlayerBND",	WebId =2033,	CustomerId = 492380,	Currency = "BND" },
        new AccountInfo{ Username = "qatestBOB",	WebId =2033,	CustomerId = 840189,	Currency = "BOB" },
        new AccountInfo{ Username = "qatestBRL1",	WebId =2033,	CustomerId = 518628,	Currency = "BRL" },
        new AccountInfo{ Username = "qatestBSD",	WebId =2033,	CustomerId = 840190,	Currency = "BSD" },
        new AccountInfo{ Username = "qatestBTN",	WebId =2033,	CustomerId = 840191,	Currency = "BTN" },
        new AccountInfo{ Username = "qatestBWP",	WebId =2033,	CustomerId = 840192,	Currency = "BWP" },
        new AccountInfo{ Username = "qatestBYN",	WebId =2033,	CustomerId = 840193,	Currency = "BYN" },
        new AccountInfo{ Username = "qatestBZD",	WebId =2033,	CustomerId = 840194,	Currency = "BZD" },
        new AccountInfo{ Username = "qatestcad",	WebId =2033,	CustomerId = 492180,	Currency = "CAD" },
        new AccountInfo{ Username = "qatestCDF",	WebId =2033,	CustomerId = 840195,	Currency = "CDF" },
        new AccountInfo{ Username = "qatestchf",	WebId =2033,	CustomerId = 492183,	Currency = "CHF" },
        new AccountInfo{ Username = "testplayerclp",	WebId =2033,	CustomerId = 622170,	Currency = "CLP" },
        new AccountInfo{ Username = "KHPlayerCNY",	WebId =2033,	CustomerId = 478771,	Currency = "CNY" },
        new AccountInfo{ Username = "Sch1038LoginPlayerCOP",	WebId =2033,	CustomerId = 737481,	Currency = "COP" },
        new AccountInfo{ Username = "qatestCRC",	WebId =2033,	CustomerId = 840196,	Currency = "CRC" },
        new AccountInfo{ Username = "qatestCUP",	WebId =2033,	CustomerId = 840197,	Currency = "CUP" },
        new AccountInfo{ Username = "qatestCVE",	WebId =2033,	CustomerId = 840198,	Currency = "CVE" },
        new AccountInfo{ Username = "qatestCZK",	WebId =2033,	CustomerId = 840199,	Currency = "CZK" },
        new AccountInfo{ Username = "qatestDJF",	WebId =2033,	CustomerId = 840200,	Currency = "DJF" },
        new AccountInfo{ Username = "qatestDKK",	WebId =2033,	CustomerId = 840201,	Currency = "DKK" },
        new AccountInfo{ Username = "qatestDOP",	WebId =2033,	CustomerId = 840202,	Currency = "DOP" },
        new AccountInfo{ Username = "JamestestDZD",	WebId =2033,	CustomerId = 1553881,	Currency = "DZD" },
        new AccountInfo{ Username = "qatestEGP",	WebId =2033,	CustomerId = 840203,	Currency = "EGP" },
        new AccountInfo{ Username = "qatestERN",	WebId =2033,	CustomerId = 840258,	Currency = "ERN" },
        new AccountInfo{ Username = "qatestETB",	WebId =2033,	CustomerId = 840204,	Currency = "ETB" },
        new AccountInfo{ Username = "qatesteur",	WebId =2033,	CustomerId = 490840,	Currency = "EUR" },
        new AccountInfo{ Username = "qatestFJD",	WebId =2033,	CustomerId = 840205,	Currency = "FJD" },
        new AccountInfo{ Username = "qatestFKP",	WebId =2033,	CustomerId = 840259,	Currency = "FKP" },
        new AccountInfo{ Username = "qatestGBP",	WebId =2033,	CustomerId = 490855,	Currency = "GBP" },
        new AccountInfo{ Username = "ttomplayerGDA",	WebId =2033,	CustomerId = 481232,	Currency = "GDA" },
        new AccountInfo{ Username = "qatestGEL",	WebId =2033,	CustomerId = 840206,	Currency = "GEL" },
        new AccountInfo{ Username = "qatestGHS",	WebId =2033,	CustomerId = 840207,	Currency = "GHS" },
        new AccountInfo{ Username = "qatestGIP",	WebId =2033,	CustomerId = 840260,	Currency = "GIP" },
        new AccountInfo{ Username = "qatestGMD",	WebId =2033,	CustomerId = 840208,	Currency = "GMD" },
        new AccountInfo{ Username = "qatestGNF",	WebId =2033,	CustomerId = 840209,	Currency = "GNF" },
        new AccountInfo{ Username = "qatestGTQ",	WebId =2033,	CustomerId = 840210,	Currency = "GTQ" },
        new AccountInfo{ Username = "qatestGYD",	WebId =2033,	CustomerId = 840211,	Currency = "GYD" },
        new AccountInfo{ Username = "CssgUatPlayerHKD",	WebId =2033,	CustomerId = 480857,	Currency = "HKD" },
        new AccountInfo{ Username = "qatestHNL",	WebId =2033,	CustomerId = 840212,	Currency = "HNL" },
        new AccountInfo{ Username = "qatestHTG",	WebId =2033,	CustomerId = 840213,	Currency = "HTG" },
        new AccountInfo{ Username = "qatestHUF",	WebId =2033,	CustomerId = 840214,	Currency = "HUF" },
        new AccountInfo{ Username = "APITTSparkIDO01",	WebId =2033,	CustomerId = 481203,	Currency = "IDO" },
        new AccountInfo{ Username = "LeoPlayer",	WebId =2033,	CustomerId = 4160,	Currency = "IDR" },
        new AccountInfo{ Username = "qatestILS",	WebId =2033,	CustomerId = 840215,	Currency = "ILS" },
        new AccountInfo{ Username = "CssgUatPlayerINR",	WebId =2033,	CustomerId = 487174,	Currency = "INR" },
        new AccountInfo{ Username = "qatestIQD",	WebId =2033,	CustomerId = 840216,	Currency = "IQD" },
        new AccountInfo{ Username = "qatestIRR",	WebId =2033,	CustomerId = 840217,	Currency = "IRR" },
        new AccountInfo{ Username = "qatestISK",	WebId =2033,	CustomerId = 840218,	Currency = "ISK" },
        new AccountInfo{ Username = "qatestJMD",	WebId =2033,	CustomerId = 840219,	Currency = "JMD" },
        new AccountInfo{ Username = "qatestJOD",	WebId =2033,	CustomerId = 840220,	Currency = "JOD" },
        new AccountInfo{ Username = "CssgUatPlayerJPY",	WebId =2033,	CustomerId = 481724,	Currency = "JPY" },
        new AccountInfo{ Username = "qatestKES",	WebId =2033,	CustomerId = 840221,	Currency = "KES" },
        new AccountInfo{ Username = "qatestKGS",	WebId =2033,	CustomerId = 840222,	Currency = "KGS" },
        new AccountInfo{ Username = "jjtestPlayerKHR",	WebId =2033,	CustomerId = 591111,	Currency = "KHR" },
        new AccountInfo{ Username = "qatestKMF",	WebId =2033,	CustomerId = 840223,	Currency = "KMF" },
        new AccountInfo{ Username = "qatestKPW",	WebId =2033,	CustomerId = 840261,	Currency = "KPW" },
        new AccountInfo{ Username = "CssgUatPlayerKRW",	WebId =2033,	CustomerId = 481723,	Currency = "KRW" },
        new AccountInfo{ Username = "qatestKWD",	WebId =2033,	CustomerId = 840224,	Currency = "KWD" },
        new AccountInfo{ Username = "qatestKYD",	WebId =2033,	CustomerId = 840225,	Currency = "KYD" },
        new AccountInfo{ Username = "qatestKZT",	WebId =2033,	CustomerId = 840226,	Currency = "KZT" },
        new AccountInfo{ Username = "qatestlak",	WebId =2033,	CustomerId = 492217,	Currency = "LAK" },
        new AccountInfo{ Username = "qatestLBP",	WebId =2033,	CustomerId = 840227,	Currency = "LBP" },
        new AccountInfo{ Username = "ssplayerLKR1",	WebId =2033,	CustomerId = 610193,	Currency = "LKR" },
        new AccountInfo{ Username = "qatestLRD",	WebId =2033,	CustomerId = 840228,	Currency = "LRD" },
        new AccountInfo{ Username = "qatestLSL",	WebId =2033,	CustomerId = 840229,	Currency = "LSL" },
        new AccountInfo{ Username = "qatestLYD",	WebId =2033,	CustomerId = 840230,	Currency = "LYD" },
        new AccountInfo{ Username = "JamestestMAD",	WebId =2033,	CustomerId = 1553924,	Currency = "MAD" },
        new AccountInfo{ Username = "qatestMDL",	WebId =2033,	CustomerId = 840231,	Currency = "MDL" },
        new AccountInfo{ Username = "qatestMGA",	WebId =2033,	CustomerId = 840232,	Currency = "MGA" },
        new AccountInfo{ Username = "qatestMKD",	WebId =2033,	CustomerId = 840233,	Currency = "MKD" },
        new AccountInfo{ Username = "qatestmmk",	WebId =2033,	CustomerId = 490858,	Currency = "MMK" },
        new AccountInfo{ Username = "Player_Mnt",	WebId =2033,	CustomerId = 734102,	Currency = "MNT" },
        new AccountInfo{ Username = "qatestMOP",	WebId =2033,	CustomerId = 840234,	Currency = "MOP" },
        new AccountInfo{ Username = "qatestMRU",	WebId =2033,	CustomerId = 840235,	Currency = "MRU" },
        new AccountInfo{ Username = "qatestMUR",	WebId =2033,	CustomerId = 840236,	Currency = "MUR" },
        new AccountInfo{ Username = "qatestMVR",	WebId =2033,	CustomerId = 840237,	Currency = "MVR" },
        new AccountInfo{ Username = "qatestMWK",	WebId =2033,	CustomerId = 840238,	Currency = "MWK" },
        new AccountInfo{ Username = "qatestmxn",	WebId =2033,	CustomerId = 521863,	Currency = "MXN" },
        new AccountInfo{ Username = "BrendorTestMYK",	WebId =2033,	CustomerId = 587443,	Currency = "MYK" },
        new AccountInfo{ Username = "CssgUatPlayerMYR",	WebId =2033,	CustomerId = 480863,	Currency = "MYR" },
        new AccountInfo{ Username = "qatestMZN",	WebId =2033,	CustomerId = 840239,	Currency = "MZN" },
        new AccountInfo{ Username = "qatestNAD",	WebId =2033,	CustomerId = 840240,	Currency = "NAD" },
        new AccountInfo{ Username = "GaryTestNGN",	WebId =2033,	CustomerId = 606186,	Currency = "NGN" },
        new AccountInfo{ Username = "qatestNIO",	WebId =2033,	CustomerId = 840241,	Currency = "NIO" },
        new AccountInfo{ Username = "qatestNOK1",	WebId =2033,	CustomerId = 518635,	Currency = "NOK" },
        new AccountInfo{ Username = "TigerTestNPR",	WebId =2033,	CustomerId = 589539,	Currency = "NPR" },
        new AccountInfo{ Username = "qatestNZD",	WebId =2033,	CustomerId = 492224,	Currency = "NZD" },
        new AccountInfo{ Username = "qatestOMR",	WebId =2033,	CustomerId = 840242,	Currency = "OMR" },
        new AccountInfo{ Username = "qatestPAB",	WebId =2033,	CustomerId = 840243,	Currency = "PAB" },
        new AccountInfo{ Username = "testplayerpen",	WebId =2033,	CustomerId = 622168,	Currency = "PEN" },
        new AccountInfo{ Username = "qatestPGK",	WebId =2033,	CustomerId = 840244,	Currency = "PGK" },
        new AccountInfo{ Username = "TestPlayerPHP001",	WebId =2033,	CustomerId = 493808,	Currency = "PHP" },
        new AccountInfo{ Username = "ssplayerPKR",	WebId =2033,	CustomerId = 610188,	Currency = "PKR" },
        new AccountInfo{ Username = "qatestPLN",	WebId =2033,	CustomerId = 840245,	Currency = "PLN" },
        new AccountInfo{ Username = "qatestPYG",	WebId =2033,	CustomerId = 840246,	Currency = "PYG" },
        new AccountInfo{ Username = "qatestQAR",	WebId =2033,	CustomerId = 840247,	Currency = "QAR" },
        new AccountInfo{ Username = "qatestRON",	WebId =2033,	CustomerId = 840248,	Currency = "RON" },
        new AccountInfo{ Username = "qatestRSD",	WebId =2033,	CustomerId = 840249,	Currency = "RSD" },
        new AccountInfo{ Username = "qatestrub",	WebId =2033,	CustomerId = 510980,	Currency = "RUB" },
        new AccountInfo{ Username = "qatestRWF",	WebId =2033,	CustomerId = 840250,	Currency = "RWF" },
        new AccountInfo{ Username = "qatestSAR",	WebId =2033,	CustomerId = 840251,	Currency = "SAR" },
        new AccountInfo{ Username = "qatestSBD",	WebId =2033,	CustomerId = 840252,	Currency = "SBD" },
        new AccountInfo{ Username = "qatestSCR",	WebId =2033,	CustomerId = 840253,	Currency = "SCR" },
        new AccountInfo{ Username = "qatestSDG",	WebId =2033,	CustomerId = 840254,	Currency = "SDG" },
        new AccountInfo{ Username = "qatestsek",	WebId =2033,	CustomerId = 492226,	Currency = "SEK" },
        new AccountInfo{ Username = "qatestSLL",	WebId =2033,	CustomerId = 840255,	Currency = "SLL" },
        new AccountInfo{ Username = "qatestSOS",	WebId =2033,	CustomerId = 840256,	Currency = "SOS" },
        new AccountInfo{ Username = "qatestSRD",	WebId =2033,	CustomerId = 840257,	Currency = "SRD" },
        new AccountInfo{ Username = "qatestSSP",	WebId =2033,	CustomerId = 840262,	Currency = "SSP" },
        new AccountInfo{ Username = "qatestSTN",	WebId =2033,	CustomerId = 840263,	Currency = "STN" },
        new AccountInfo{ Username = "qatestSYP",	WebId =2033,	CustomerId = 840264,	Currency = "SYP" },
        new AccountInfo{ Username = "qatestSZL",	WebId =2033,	CustomerId = 840267,	Currency = "SZL" },
        new AccountInfo{ Username = "khplayerthb",	WebId =2033,	CustomerId = 480229,	Currency = "THB" },
        new AccountInfo{ Username = "qatestTJS",	WebId =2033,	CustomerId = 840268,	Currency = "TJS" },
        new AccountInfo{ Username = "apiwesker03",	WebId =2033,	CustomerId = 476632,	Currency = "TMP" },
        new AccountInfo{ Username = "qatestTMT",	WebId =2033,	CustomerId = 840269,	Currency = "TMT" },
        new AccountInfo{ Username = "qatestTND",	WebId =2033,	CustomerId = 840270,	Currency = "TND" },
        new AccountInfo{ Username = "qatestTOP",	WebId =2033,	CustomerId = 840271,	Currency = "TOP" },
        new AccountInfo{ Username = "ssplayerTRY",	WebId =2033,	CustomerId = 610186,	Currency = "TRY" },
        new AccountInfo{ Username = "qatestTTD",	WebId =2033,	CustomerId = 840272,	Currency = "TTD" },
        new AccountInfo{ Username = "qatestTZS",	WebId =2033,	CustomerId = 840273,	Currency = "TZS" },
        new AccountInfo{ Username = "qatestUAH",	WebId =2033,	CustomerId = 840274,	Currency = "UAH" },
        new AccountInfo{ Username = "qatestUCC",	WebId =2033,	CustomerId = 718450,	Currency = "UCC" },
        new AccountInfo{ Username = "qatestUGX",	WebId =2033,	CustomerId = 840275,	Currency = "UGX" },
        new AccountInfo{ Username = "TestPlayerTim001",	WebId =2033,	CustomerId = 461977,	Currency = "USD" },
        new AccountInfo{ Username = "qatestUYU",	WebId =2033,	CustomerId = 840276,	Currency = "UYU" },
        new AccountInfo{ Username = "qatestUZS",	WebId =2033,	CustomerId = 840277,	Currency = "UZS" },
        new AccountInfo{ Username = "Sch1038LoginPlayerVES",	WebId =2033,	CustomerId = 737524,	Currency = "VES" },
        new AccountInfo{ Username = "test_strong_2",	WebId =2033,	CustomerId = 448874,	Currency = "VND" },
        new AccountInfo{ Username = "APITTSparkVNO01",	WebId =2033,	CustomerId = 481198,	Currency = "VNO" },
        new AccountInfo{ Username = "qatestVUV",	WebId =2033,	CustomerId = 840265,	Currency = "VUV" },
        new AccountInfo{ Username = "qatestWST",	WebId =2033,	CustomerId = 840266,	Currency = "WST" },
        new AccountInfo{ Username = "qatestXAF",	WebId =2033,	CustomerId = 840278,	Currency = "XAF" },
        new AccountInfo{ Username = "qatestXCD",	WebId =2033,	CustomerId = 840279,	Currency = "XCD" },
        new AccountInfo{ Username = "qatestXOF",	WebId =2033,	CustomerId = 840280,	Currency = "XOF" },
        new AccountInfo{ Username = "qatestXPF",	WebId =2033,	CustomerId = 840281,	Currency = "XPF" },
        new AccountInfo{ Username = "qatestYER",	WebId =2033,	CustomerId = 840282,	Currency = "YER" },
        new AccountInfo{ Username = "qatestZAR1",	WebId =2033,	CustomerId = 518640,	Currency = "ZAR" },
        new AccountInfo{ Username = "qatestZMW",	WebId =2033,	CustomerId = 840283,	Currency = "ZMW" },
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