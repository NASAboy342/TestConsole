using System;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using DocumentFormat.OpenXml.InkML;
using Newtonsoft.Json;
using TestConsole.Helper;
using TestConsole.model;

namespace TestConsole.Programs;

public class UtopiaTestLogin
{
    private readonly int _providerId = 1099;
    private readonly string _utopiaUrl = "https://capi-uat-indialotto.csmc-api.com";
    private readonly List<string> providerSupportedCurrency = new List<string> { "TMP", "INR", "AED", "AUD", "BDT", "BND", "CNY", "EGP", "ETB", "EUR", "GBP", "HKD", "IDR", "IDO", "ILS", "JPY", "KES", "KHR", "KZT", "LAK", "LKR", "MAD", "MMK", "MMO", "MVR", "MYR", "NGN", "NOK", "NPR", "OMR", "PGK", "PHP", "PKR", "RUB", "SAR", "SEK", "THB", "TND", "TRY", "USDT", "UZS", "VND", "VNO", "ZAR", "ZMW", "XOF" };
    private readonly object _lockSetLoginResult = new object();


    public UtopiaTestLogin(int providerId = 1099, string utopiaUrl = null)
    {
        _providerId = providerId;
        if (!string.IsNullOrEmpty(utopiaUrl))
        {
            _utopiaUrl = utopiaUrl;
        }
    }
    private readonly List<AccountInfo> accountInfosOnDemo = new()
    {
        new AccountInfo{ Username = "qatestaed",    WebId =2033,    CustomerId = 521754,    Currency = "AED" },
        new AccountInfo{ Username = "qatestAFN",    WebId =2033,    CustomerId = 840175,    Currency = "AFN" },
        new AccountInfo{ Username = "qatestALL",    WebId =2033,    CustomerId = 840176,    Currency = "ALL" },
        new AccountInfo{ Username = "qatestAMD",    WebId =2033,    CustomerId = 840177,    Currency = "AMD" },
        new AccountInfo{ Username = "qatestANG",    WebId =2033,    CustomerId = 840178,    Currency = "ANG" },
        new AccountInfo{ Username = "qatestAOA",    WebId =2033,    CustomerId = 840179,    Currency = "AOA" },
        new AccountInfo{ Username = "qatestARS",    WebId =2033,    CustomerId = 840180,    Currency = "ARS" },
        new AccountInfo{ Username = "qatestaud",    WebId =2033,    CustomerId = 490836,    Currency = "AUD" },
        new AccountInfo{ Username = "qatestAWG",    WebId =2033,    CustomerId = 840181,    Currency = "AWG" },
        new AccountInfo{ Username = "qatestAZN",    WebId =2033,    CustomerId = 840182,    Currency = "AZN" },
        new AccountInfo{ Username = "qatestBAM",    WebId =2033,    CustomerId = 840183,    Currency = "BAM" },
        new AccountInfo{ Username = "qatestBBD",    WebId =2033,    CustomerId = 840184,    Currency = "BBD" },
        new AccountInfo{ Username = "CssgUatPlayerBDT", WebId =2033,    CustomerId = 492378,    Currency = "BDT" },
        new AccountInfo{ Username = "qatestBGN",    WebId =2033,    CustomerId = 840185,    Currency = "BGN" },
        new AccountInfo{ Username = "qatestBHD",    WebId =2033,    CustomerId = 840186,    Currency = "BHD" },
        new AccountInfo{ Username = "qatestBIF",    WebId =2033,    CustomerId = 840187,    Currency = "BIF" },
        new AccountInfo{ Username = "qatestBMD",    WebId =2033,    CustomerId = 840188,    Currency = "BMD" },
        new AccountInfo{ Username = "CssgUatPlayerBND", WebId =2033,    CustomerId = 492380,    Currency = "BND" },
        new AccountInfo{ Username = "qatestBOB",    WebId =2033,    CustomerId = 840189,    Currency = "BOB" },
        new AccountInfo{ Username = "qatestBRL1",   WebId =2033,    CustomerId = 518628,    Currency = "BRL" },
        new AccountInfo{ Username = "qatestBSD",    WebId =2033,    CustomerId = 840190,    Currency = "BSD" },
        new AccountInfo{ Username = "qatestBTN",    WebId =2033,    CustomerId = 840191,    Currency = "BTN" },
        new AccountInfo{ Username = "qatestBWP",    WebId =2033,    CustomerId = 840192,    Currency = "BWP" },
        new AccountInfo{ Username = "qatestBYN",    WebId =2033,    CustomerId = 840193,    Currency = "BYN" },
        new AccountInfo{ Username = "qatestBZD",    WebId =2033,    CustomerId = 840194,    Currency = "BZD" },
        new AccountInfo{ Username = "qatestcad",    WebId =2033,    CustomerId = 492180,    Currency = "CAD" },
        new AccountInfo{ Username = "qatestCDF",    WebId =2033,    CustomerId = 840195,    Currency = "CDF" },
        new AccountInfo{ Username = "qatestchf",    WebId =2033,    CustomerId = 492183,    Currency = "CHF" },
        new AccountInfo{ Username = "testplayerclp",    WebId =2033,    CustomerId = 622170,    Currency = "CLP" },
        new AccountInfo{ Username = "KHPlayerCNY",  WebId =2033,    CustomerId = 478771,    Currency = "CNY" },
        new AccountInfo{ Username = "Sch1038LoginPlayerCOP",    WebId =2033,    CustomerId = 737481,    Currency = "COP" },
        new AccountInfo{ Username = "qatestCRC",    WebId =2033,    CustomerId = 840196,    Currency = "CRC" },
        new AccountInfo{ Username = "qatestCUP",    WebId =2033,    CustomerId = 840197,    Currency = "CUP" },
        new AccountInfo{ Username = "qatestCVE",    WebId =2033,    CustomerId = 840198,    Currency = "CVE" },
        new AccountInfo{ Username = "qatestCZK",    WebId =2033,    CustomerId = 840199,    Currency = "CZK" },
        new AccountInfo{ Username = "qatestDJF",    WebId =2033,    CustomerId = 840200,    Currency = "DJF" },
        new AccountInfo{ Username = "qatestDKK",    WebId =2033,    CustomerId = 840201,    Currency = "DKK" },
        new AccountInfo{ Username = "qatestDOP",    WebId =2033,    CustomerId = 840202,    Currency = "DOP" },
        new AccountInfo{ Username = "JamestestDZD", WebId =2033,    CustomerId = 1553881,   Currency = "DZD" },
        new AccountInfo{ Username = "qatestEGP",    WebId =2033,    CustomerId = 840203,    Currency = "EGP" },
        new AccountInfo{ Username = "qatestERN",    WebId =2033,    CustomerId = 840258,    Currency = "ERN" },
        new AccountInfo{ Username = "qatestETB",    WebId =2033,    CustomerId = 840204,    Currency = "ETB" },
        new AccountInfo{ Username = "qatesteur",    WebId =2033,    CustomerId = 490840,    Currency = "EUR" },
        new AccountInfo{ Username = "qatestFJD",    WebId =2033,    CustomerId = 840205,    Currency = "FJD" },
        new AccountInfo{ Username = "qatestFKP",    WebId =2033,    CustomerId = 840259,    Currency = "FKP" },
        new AccountInfo{ Username = "qatestGBP",    WebId =2033,    CustomerId = 490855,    Currency = "GBP" },
        new AccountInfo{ Username = "ttomplayerGDA",    WebId =2033,    CustomerId = 481232,    Currency = "GDA" },
        new AccountInfo{ Username = "qatestGEL",    WebId =2033,    CustomerId = 840206,    Currency = "GEL" },
        new AccountInfo{ Username = "qatestGHS",    WebId =2033,    CustomerId = 840207,    Currency = "GHS" },
        new AccountInfo{ Username = "qatestGIP",    WebId =2033,    CustomerId = 840260,    Currency = "GIP" },
        new AccountInfo{ Username = "qatestGMD",    WebId =2033,    CustomerId = 840208,    Currency = "GMD" },
        new AccountInfo{ Username = "qatestGNF",    WebId =2033,    CustomerId = 840209,    Currency = "GNF" },
        new AccountInfo{ Username = "qatestGTQ",    WebId =2033,    CustomerId = 840210,    Currency = "GTQ" },
        new AccountInfo{ Username = "qatestGYD",    WebId =2033,    CustomerId = 840211,    Currency = "GYD" },
        new AccountInfo{ Username = "CssgUatPlayerHKD", WebId =2033,    CustomerId = 480857,    Currency = "HKD" },
        new AccountInfo{ Username = "qatestHNL",    WebId =2033,    CustomerId = 840212,    Currency = "HNL" },
        new AccountInfo{ Username = "qatestHTG",    WebId =2033,    CustomerId = 840213,    Currency = "HTG" },
        new AccountInfo{ Username = "qatestHUF",    WebId =2033,    CustomerId = 840214,    Currency = "HUF" },
        new AccountInfo{ Username = "APITTSparkIDO01",  WebId =2033,    CustomerId = 481203,    Currency = "IDO" },
        new AccountInfo{ Username = "LeoPlayer",    WebId =2033,    CustomerId = 4160,  Currency = "IDR" },
        new AccountInfo{ Username = "qatestILS",    WebId =2033,    CustomerId = 840215,    Currency = "ILS" },
        new AccountInfo{ Username = "CssgUatPlayerINR", WebId =2033,    CustomerId = 487174,    Currency = "INR" },
        new AccountInfo{ Username = "qatestIQD",    WebId =2033,    CustomerId = 840216,    Currency = "IQD" },
        new AccountInfo{ Username = "qatestIRR",    WebId =2033,    CustomerId = 840217,    Currency = "IRR" },
        new AccountInfo{ Username = "qatestISK",    WebId =2033,    CustomerId = 840218,    Currency = "ISK" },
        new AccountInfo{ Username = "qatestJMD",    WebId =2033,    CustomerId = 840219,    Currency = "JMD" },
        new AccountInfo{ Username = "qatestJOD",    WebId =2033,    CustomerId = 840220,    Currency = "JOD" },
        new AccountInfo{ Username = "CssgUatPlayerJPY", WebId =2033,    CustomerId = 481724,    Currency = "JPY" },
        new AccountInfo{ Username = "qatestKES",    WebId =2033,    CustomerId = 840221,    Currency = "KES" },
        new AccountInfo{ Username = "qatestKGS",    WebId =2033,    CustomerId = 840222,    Currency = "KGS" },
        new AccountInfo{ Username = "jjtestPlayerKHR",  WebId =2033,    CustomerId = 591111,    Currency = "KHR" },
        new AccountInfo{ Username = "qatestKMF",    WebId =2033,    CustomerId = 840223,    Currency = "KMF" },
        new AccountInfo{ Username = "qatestKPW",    WebId =2033,    CustomerId = 840261,    Currency = "KPW" },
        new AccountInfo{ Username = "CssgUatPlayerKRW", WebId =2033,    CustomerId = 481723,    Currency = "KRW" },
        new AccountInfo{ Username = "qatestKWD",    WebId =2033,    CustomerId = 840224,    Currency = "KWD" },
        new AccountInfo{ Username = "qatestKYD",    WebId =2033,    CustomerId = 840225,    Currency = "KYD" },
        new AccountInfo{ Username = "qatestKZT",    WebId =2033,    CustomerId = 840226,    Currency = "KZT" },
        new AccountInfo{ Username = "qatestlak",    WebId =2033,    CustomerId = 492217,    Currency = "LAK" },
        new AccountInfo{ Username = "qatestLBP",    WebId =2033,    CustomerId = 840227,    Currency = "LBP" },
        new AccountInfo{ Username = "ssplayerLKR1", WebId =2033,    CustomerId = 610193,    Currency = "LKR" },
        new AccountInfo{ Username = "qatestLRD",    WebId =2033,    CustomerId = 840228,    Currency = "LRD" },
        new AccountInfo{ Username = "qatestLSL",    WebId =2033,    CustomerId = 840229,    Currency = "LSL" },
        new AccountInfo{ Username = "qatestLYD",    WebId =2033,    CustomerId = 840230,    Currency = "LYD" },
        new AccountInfo{ Username = "JamestestMAD", WebId =2033,    CustomerId = 1553924,   Currency = "MAD" },
        new AccountInfo{ Username = "qatestMDL",    WebId =2033,    CustomerId = 840231,    Currency = "MDL" },
        new AccountInfo{ Username = "qatestMGA",    WebId =2033,    CustomerId = 840232,    Currency = "MGA" },
        new AccountInfo{ Username = "qatestMKD",    WebId =2033,    CustomerId = 840233,    Currency = "MKD" },
        new AccountInfo{ Username = "qatestmmk",    WebId =2033,    CustomerId = 490858,    Currency = "MMK" },
        new AccountInfo{ Username = "Player_Mnt",   WebId =2033,    CustomerId = 734102,    Currency = "MNT" },
        new AccountInfo{ Username = "qatestMOP",    WebId =2033,    CustomerId = 840234,    Currency = "MOP" },
        new AccountInfo{ Username = "qatestMRU",    WebId =2033,    CustomerId = 840235,    Currency = "MRU" },
        new AccountInfo{ Username = "qatestMUR",    WebId =2033,    CustomerId = 840236,    Currency = "MUR" },
        new AccountInfo{ Username = "qatestMVR",    WebId =2033,    CustomerId = 840237,    Currency = "MVR" },
        new AccountInfo{ Username = "qatestMWK",    WebId =2033,    CustomerId = 840238,    Currency = "MWK" },
        new AccountInfo{ Username = "qatestmxn",    WebId =2033,    CustomerId = 521863,    Currency = "MXN" },
        new AccountInfo{ Username = "BrendorTestMYK",   WebId =2033,    CustomerId = 587443,    Currency = "MYK" },
        new AccountInfo{ Username = "CssgUatPlayerMYR", WebId =2033,    CustomerId = 480863,    Currency = "MYR" },
        new AccountInfo{ Username = "qatestMZN",    WebId =2033,    CustomerId = 840239,    Currency = "MZN" },
        new AccountInfo{ Username = "qatestNAD",    WebId =2033,    CustomerId = 840240,    Currency = "NAD" },
        new AccountInfo{ Username = "GaryTestNGN",  WebId =2033,    CustomerId = 606186,    Currency = "NGN" },
        new AccountInfo{ Username = "qatestNIO",    WebId =2033,    CustomerId = 840241,    Currency = "NIO" },
        new AccountInfo{ Username = "qatestNOK1",   WebId =2033,    CustomerId = 518635,    Currency = "NOK" },
        new AccountInfo{ Username = "TigerTestNPR", WebId =2033,    CustomerId = 589539,    Currency = "NPR" },
        new AccountInfo{ Username = "qatestNZD",    WebId =2033,    CustomerId = 492224,    Currency = "NZD" },
        new AccountInfo{ Username = "qatestOMR",    WebId =2033,    CustomerId = 840242,    Currency = "OMR" },
        new AccountInfo{ Username = "qatestPAB",    WebId =2033,    CustomerId = 840243,    Currency = "PAB" },
        new AccountInfo{ Username = "testplayerpen",    WebId =2033,    CustomerId = 622168,    Currency = "PEN" },
        new AccountInfo{ Username = "qatestPGK",    WebId =2033,    CustomerId = 840244,    Currency = "PGK" },
        new AccountInfo{ Username = "TestPlayerPHP001", WebId =2033,    CustomerId = 493808,    Currency = "PHP" },
        new AccountInfo{ Username = "ssplayerPKR",  WebId =2033,    CustomerId = 610188,    Currency = "PKR" },
        new AccountInfo{ Username = "qatestPLN",    WebId =2033,    CustomerId = 840245,    Currency = "PLN" },
        new AccountInfo{ Username = "qatestPYG",    WebId =2033,    CustomerId = 840246,    Currency = "PYG" },
        new AccountInfo{ Username = "qatestQAR",    WebId =2033,    CustomerId = 840247,    Currency = "QAR" },
        new AccountInfo{ Username = "qatestRON",    WebId =2033,    CustomerId = 840248,    Currency = "RON" },
        new AccountInfo{ Username = "qatestRSD",    WebId =2033,    CustomerId = 840249,    Currency = "RSD" },
        new AccountInfo{ Username = "qatestrub",    WebId =2033,    CustomerId = 510980,    Currency = "RUB" },
        new AccountInfo{ Username = "qatestRWF",    WebId =2033,    CustomerId = 840250,    Currency = "RWF" },
        new AccountInfo{ Username = "qatestSAR",    WebId =2033,    CustomerId = 840251,    Currency = "SAR" },
        new AccountInfo{ Username = "qatestSBD",    WebId =2033,    CustomerId = 840252,    Currency = "SBD" },
        new AccountInfo{ Username = "qatestSCR",    WebId =2033,    CustomerId = 840253,    Currency = "SCR" },
        new AccountInfo{ Username = "qatestSDG",    WebId =2033,    CustomerId = 840254,    Currency = "SDG" },
        new AccountInfo{ Username = "qatestsek",    WebId =2033,    CustomerId = 492226,    Currency = "SEK" },
        new AccountInfo{ Username = "qatestSLL",    WebId =2033,    CustomerId = 840255,    Currency = "SLL" },
        new AccountInfo{ Username = "qatestSOS",    WebId =2033,    CustomerId = 840256,    Currency = "SOS" },
        new AccountInfo{ Username = "qatestSRD",    WebId =2033,    CustomerId = 840257,    Currency = "SRD" },
        new AccountInfo{ Username = "qatestSSP",    WebId =2033,    CustomerId = 840262,    Currency = "SSP" },
        new AccountInfo{ Username = "qatestSTN",    WebId =2033,    CustomerId = 840263,    Currency = "STN" },
        new AccountInfo{ Username = "qatestSYP",    WebId =2033,    CustomerId = 840264,    Currency = "SYP" },
        new AccountInfo{ Username = "qatestSZL",    WebId =2033,    CustomerId = 840267,    Currency = "SZL" },
        new AccountInfo{ Username = "khplayerthb",  WebId =2033,    CustomerId = 480229,    Currency = "THB" },
        new AccountInfo{ Username = "qatestTJS",    WebId =2033,    CustomerId = 840268,    Currency = "TJS" },
        new AccountInfo{ Username = "apiwesker03",  WebId =2033,    CustomerId = 476632,    Currency = "TMP" },
        new AccountInfo{ Username = "qatestTMT",    WebId =2033,    CustomerId = 840269,    Currency = "TMT" },
        new AccountInfo{ Username = "qatestTND",    WebId =2033,    CustomerId = 840270,    Currency = "TND" },
        new AccountInfo{ Username = "qatestTOP",    WebId =2033,    CustomerId = 840271,    Currency = "TOP" },
        new AccountInfo{ Username = "ssplayerTRY",  WebId =2033,    CustomerId = 610186,    Currency = "TRY" },
        new AccountInfo{ Username = "qatestTTD",    WebId =2033,    CustomerId = 840272,    Currency = "TTD" },
        new AccountInfo{ Username = "qatestTZS",    WebId =2033,    CustomerId = 840273,    Currency = "TZS" },
        new AccountInfo{ Username = "qatestUAH",    WebId =2033,    CustomerId = 840274,    Currency = "UAH" },
        new AccountInfo{ Username = "qatestUCC",    WebId =2033,    CustomerId = 718450,    Currency = "UCC" },
        new AccountInfo{ Username = "qatestUGX",    WebId =2033,    CustomerId = 840275,    Currency = "UGX" },
        new AccountInfo{ Username = "TestPlayerTim001", WebId =2033,    CustomerId = 461977,    Currency = "USD" },
        new AccountInfo{ Username = "qatestUYU",    WebId =2033,    CustomerId = 840276,    Currency = "UYU" },
        new AccountInfo{ Username = "qatestUZS",    WebId =2033,    CustomerId = 840277,    Currency = "UZS" },
        new AccountInfo{ Username = "Sch1038LoginPlayerVES",    WebId =2033,    CustomerId = 737524,    Currency = "VES" },
        new AccountInfo{ Username = "test_strong_2",    WebId =2033,    CustomerId = 448874,    Currency = "VND" },
        new AccountInfo{ Username = "APITTSparkVNO01",  WebId =2033,    CustomerId = 481198,    Currency = "VNO" },
        new AccountInfo{ Username = "qatestVUV",    WebId =2033,    CustomerId = 840265,    Currency = "VUV" },
        new AccountInfo{ Username = "qatestWST",    WebId =2033,    CustomerId = 840266,    Currency = "WST" },
        new AccountInfo{ Username = "qatestXAF",    WebId =2033,    CustomerId = 840278,    Currency = "XAF" },
        new AccountInfo{ Username = "qatestXCD",    WebId =2033,    CustomerId = 840279,    Currency = "XCD" },
        new AccountInfo{ Username = "qatestXOF",    WebId =2033,    CustomerId = 840280,    Currency = "XOF" },
        new AccountInfo{ Username = "qatestXPF",    WebId =2033,    CustomerId = 840281,    Currency = "XPF" },
        new AccountInfo{ Username = "qatestYER",    WebId =2033,    CustomerId = 840282,    Currency = "YER" },
        new AccountInfo{ Username = "qatestZAR1",   WebId =2033,    CustomerId = 518640,    Currency = "ZAR" },
        new AccountInfo{ Username = "qatestZMW",    WebId =2033,    CustomerId = 840283,    Currency = "ZMW" },
    };
    internal async Task TestAllCurrencyByGameDemo(int gameId)
    {
        foreach (var account in accountInfosOnDemo)
        {
            if (!providerSupportedCurrency.Contains(account.Currency))
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
    public async Task TestAllGameAndAllOfItsCurrencyDemo(int startFromGameId = 0)
    {
        var arpiaApiHelper = new ArpiaApiHelper();
        var gameListFromArpia = await arpiaApiHelper.GetAllGamesAsync(_providerId);
        // var gameListResponse = await GetAllGameFromMirana();
        var loginResults = new List<TestLoginResult>();
        foreach (var game in gameListFromArpia.Data.Games.Where(g => startFromGameId == 0 || g.GameId >= startFromGameId).OrderBy(g => g.GameId))
        {
            var tasks = new List<Task>();
            Console.WriteLine($"Testing GameId: {game.GameId}=====================================================================================");
            foreach (var currency in game.SupportedCurrencies)
            {
                tasks.Add(TesGameAndItsCurrencyDemo(currency, game, loginResults));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("----------------------------------------------------------------------------------------------\n");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
        Console.WriteLine("saving file...");
        ExcelHelper.WriteDataTableToExcel(DataTableHelper.ToDataTable(loginResults), $"/Users/pinsopheaktra/Downloads/LoginAllGamesAllCurrencies_UtopiaDemo{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        Console.WriteLine("All done!");
    }

    private async Task TesGameAndItsCurrencyDemo(string currency, ArpiaGame game, List<TestLoginResult> loginResults)
    {
        var outputString = new StringBuilder();
        if (currency == null)
        {
            outputString.AppendLine($"No currency found for GameId {game.GameId}\n");
            SetLoginResult(loginResults, "Failed", game.GameId, currency, "", "No currency found", 1, game.GameCode, game.Provider);
            return;
        }
        AccountInfo? account = accountInfosOnDemo.Find(a => a.Currency == currency);
        if (account == null)
        {
            outputString.AppendLine($"No account found for currency {currency}\n");
            SetLoginResult(loginResults, "Failed", game.GameId, currency, "", "No account found", 1, game.GameCode, game.Provider);
            return;
        }
        var username = $"{account.WebId}SBONS{account.CustomerId}";
        var customerId = account.CustomerId;
        var accountId = $"{account.WebId}yy_{account.Username}";
        try
        {
            var loginResponse = await CallLogin(username, customerId, accountId, game.GameId, currency);
            outputString.AppendLine($"Testing GameId: {game.GameId} for Currency: {currency}=====================================================================\n");
            if (loginResponse.IsSuccess && !loginResponse.Result.Contains("mainopia-uat", StringComparison.OrdinalIgnoreCase))
            {
                outputString.AppendLine($"Login Success! For GameId {game.GameId} Launch URL: {loginResponse.Result}\n");
                SetLoginResult(loginResults, "Success", game.GameId, currency, loginResponse.Result, "", 0, game.GameCode, game.Provider);
            }
            else
            {
                outputString.AppendLine($"Login Failed! For GameId {game.GameId} ErrorCode: {loginResponse.ErrorCode}, ErrorMessage: {loginResponse.ErrorMessage} Launch URL: {loginResponse.Result}\n");
                SetLoginResult(loginResults, "Failed", game.GameId, currency, loginResponse.Result, loginResponse.ErrorMessage, loginResponse.ErrorCode, game.GameCode, game.Provider);
            }
        }
        catch (Exception ex)
        {
            outputString.AppendLine($"Exception for GameId {game.GameId} Currency {currency}: {ex.Message}\n");
            SetLoginResult(loginResults, "Exception", game.GameId, currency, "", ex.Message, 1, game.GameCode, game.Provider);
        }

        outputString.AppendLine("----------------------------------------------------------------------------------------------\n");
        Console.WriteLine(outputString.ToString());
    }

    private void SetLoginResult(List<TestLoginResult> loginResults, string status , int gameID, string? currency, string url, string message, int errorCode, string gameCode, string provider)
    {
        lock (_lockSetLoginResult)
        {
            loginResults.Add(new TestLoginResult
            {
                GameId = gameID,
                GameCode = gameCode,
                Provider = provider,
                Currency = currency,
                ErrorMessage = message,
                ErrorCode = errorCode,
                LaunchUrl = url,
                Status = status
            });
        }
    }

    public async Task TestAllGameByOneOfItsCurrencyDemo()
    {
        var gameListResponse = await GetAllGameFromMirana();
        foreach (var game in gameListResponse.SeamlessGameProviderGames)
        {
            var currency = game.SupportedCurrencies.FirstOrDefault();
            if (currency == null)
            {
                Console.WriteLine($"No currency found for GameId {game.GameID}");
                continue;
            }
            var account = accountInfosOnDemo.Find(a => a.Currency == currency);
            if (account == null)
            {
                Console.WriteLine($"No account found for currency {currency}");
                continue;
            }
            var username = $"{account.WebId}SBONS{account.CustomerId}";
            var customerId = account.CustomerId;
            var accountId = $"{account.WebId}yy_{account.Username}";
            try
            {
                var loginResponse = await CallLogin(username, customerId, accountId, game.GameID, currency);
                Console.WriteLine($"Testing GameId: {game.GameID} for Currency: {currency}=====================================================================");
                if (loginResponse.IsSuccess && !loginResponse.Result.Contains("mainopia-uat", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Login Success! For GameId {game.GameID} Launch URL: {loginResponse.Result}");
                }
                else
                {
                    Console.WriteLine($"Login Failed! For GameId {game.GameID} ErrorCode: {loginResponse.ErrorCode}, ErrorMessage: {loginResponse.ErrorMessage} Launch URL: {loginResponse.Result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception for GameId {game.GameID} Currency {currency}: {ex.Message}");
            }

            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }
        Console.WriteLine("All done!");
    }

    internal async Task TestAllGameByCurrencyDemo(string currency)
    {
        foreach (var gameId in await GetGameIds())
        {
            var account = accountInfosOnDemo.Find(a => a.Currency == currency);
            if (account == null)
            {
                Console.WriteLine($"No account found for currency {currency}");
                continue;
            }
            var username = $"{account.WebId}SBONS{account.CustomerId}";
            var customerId = account.CustomerId;
            var accountId = $"{account.WebId}yy_{account.Username}";

            var loginResponse = await CallLogin(username, customerId, accountId, gameId, currency);
            Console.WriteLine($"Testing GameId: {gameId}    for Currency: {currency}=====================================================================");
            if (loginResponse.IsSuccess)
            {
                Console.WriteLine($"Login Success!  For GameId {gameId} Launch URL: {loginResponse.Result}");
            }
            else
            {
                Console.WriteLine($"Login Failed!   ErrorCode: {loginResponse.ErrorCode}, ErrorMessage: {loginResponse.ErrorMessage}");
            }
            Console.WriteLine("----------------------------------------------------------------------------------------------");
        }
        Console.WriteLine("All done!");
    }

    public async Task<List<int>> GetGameIds()
    {
        var gameListResponse = await GetAllGameFromMirana();
        return gameListResponse.SeamlessGameProviderGames.Select(g => g.GameID).ToList();
    }

    private async Task<MiranaGamelist> GetAllGameFromMirana()
    {
        var url = "https://ex-api-demo-yy.568win.com/web-root/restricted/information/get-game-list.aspx";
        var request = new MiranaGetGameListRequest
        {
            CompanyKey = "",
            GpId = _providerId,
            IsGetAll = false,
            ServerId = ""
        };
        using var client = new HttpClient();
        var httpResponse = await client.PostAsJsonAsync(url, request);
        var responseString = await httpResponse.Content.ReadAsStringAsync();
        var gameListResponse = JsonConvert.DeserializeObject<MiranaGamelist>(responseString);
        return gameListResponse;
    }

    private async Task<LoginResponse> CallLogin(string username, int customerId, string accountId, int gameId, string currency)
    {
        var loginRequest = new LoginRequest()
        {
            User = username,
            ProcessLoginModel = new ProcessLoginModel
            {
                GameId = gameId,
                GpId = _providerId,
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
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_utopiaUrl}/Provider/Login");
        var content = new StringContent(JsonConvert.SerializeObject(loginRequest), null, "application/json");
        request.Content = content;
        var response = await client.SendAsync(request);
        var loginResponsestring = await response.Content.ReadAsStringAsync();
        var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(loginResponsestring);
        return loginResponse;
    }
}

internal class UtopiaApiHelper
{
    public UtopiaApiHelper()
    {
    }
}

internal class MiranaGetGameListRequest
{
    public int GpId { get; set; }
    public bool IsGetAll { get; set; }
    public string CompanyKey { get; set; }
    public string ServerId { get; set; }
}

public class TestLoginResult
{
    public string Status { get; set; } = "";
    public int GameId { get; set; } = 0;
    public string GameCode { get; set; } = "";
    public string Provider { get; set; } = "";
    public string Currency { get; set; } = "";
    public string LaunchUrl { get; set; } = "";
    public string ErrorMessage { get; set; } = "";
    public int ErrorCode { get; set; } = 0;
}