using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Programs
{
    public class ApGaming : IProgram
    {
        private readonly string INIT_VECTOR = "RandomInitVector";
        public async Task TestFinancial()
        {
            List<ApGamingBetDetail> data = JsonConvert.DeserializeObject<List<ApGamingBetDetail>>(
                @"[
    {
        ""wagerId"": 2201407821,
        ""eventId"": 1606044108,
        ""eventName"": ""Arsenal-vs-Real Madrid"",
        ""parentEventName"": null,
        ""headToHead"": null,
        ""wagerDateFm"": ""2025-04-07 02:14:50"",
        ""eventDateFm"": ""2025-04-08 15:00:00"",
        ""settleDateFm"": null,
        ""resettleDateFm"": null,
        ""status"": ""OPEN"",
        ""homeTeam"": ""Arsenal"",
        ""awayTeam"": ""Real Madrid"",
        ""selection"": ""Under"",
        ""handicap"": 1.25,
        ""odds"": -2.00,
        ""oddsFormat"": 3,
        ""betType"": 3,
        ""league"": ""UEFA - Champions League"",
        ""leagueId"": 2627,
        ""stake"": 1.11,
        ""sportId"": 29,
        ""sport"": ""Soccer"",
        ""currencyCode"": ""IDR"",
        ""inplayScore"": """",
        ""inPlay"": false,
        ""homePitcher"": null,
        ""awayPitcher"": null,
        ""homePitcherName"": null,
        ""awayPitcherName"": null,
        ""period"": 1,
        ""cancellationStatus"": null,
        ""parlaySelections"": [],
        ""category"": null,
        ""toWin"": 1.1100000,
        ""toRisk"": 2.2200000,
        ""product"": ""SB"",
        ""isResettle"": null,
        ""parlayMixOdds"": -2.0000000,
        ""parlayFinalOdds"": -2.0000000,
        ""wagerType"": ""single"",
        ""competitors"": [],
        ""userCode"": ""AE60100002"",
        ""loginId"": ""2033sbons493727"",
        ""winLoss"": 0.00,
        ""turnover"": 0.00,
        ""scores"": [],
        ""result"": null,
        ""volume"": 1.11,
        ""view"": ""D-Compact""
    },
    {
        ""wagerId"": 2201406781,
        ""eventId"": 1607076542,
        ""eventName"": ""Hapoel Haifa BC-vs-Hapoel Galil Elion"",
        ""parentEventName"": null,
        ""headToHead"": null,
        ""wagerDateFm"": ""2025-04-07 01:01:40"",
        ""eventDateFm"": ""2025-04-07 11:30:00"",
        ""settleDateFm"": null,
        ""resettleDateFm"": null,
        ""status"": ""OPEN"",
        ""homeTeam"": ""Hapoel Haifa BC"",
        ""awayTeam"": ""Hapoel Galil Elion"",
        ""selection"": ""Hapoel Galil Elion"",
        ""handicap"": -3.00,
        ""odds"": -1.04,
        ""oddsFormat"": 3,
        ""betType"": 2,
        ""league"": ""Israel - Premier League"",
        ""leagueId"": 462,
        ""stake"": 2.13,
        ""sportId"": 4,
        ""sport"": ""Basketball"",
        ""currencyCode"": ""IDR"",
        ""inplayScore"": """",
        ""inPlay"": false,
        ""homePitcher"": null,
        ""awayPitcher"": null,
        ""homePitcherName"": null,
        ""awayPitcherName"": null,
        ""period"": 1,
        ""cancellationStatus"": null,
        ""parlaySelections"": [],
        ""category"": null,
        ""toWin"": 2.1300000,
        ""toRisk"": 2.2200000,
        ""product"": ""SB"",
        ""isResettle"": null,
        ""parlayMixOdds"": -1.0400000,
        ""parlayFinalOdds"": -1.0400000,
        ""wagerType"": ""single"",
        ""competitors"": [],
        ""userCode"": ""AE60100002"",
        ""loginId"": ""2033sbons493727"",
        ""winLoss"": 0.00,
        ""turnover"": 0.00,
        ""scores"": [],
        ""result"": null,
        ""volume"": 2.13,
        ""view"": ""D-Compact""
    }
]"
                );

            Console.WriteLine(JsonConvert.SerializeObject(data));
            //Console.WriteLine(EncodeToken(GetTimestamp().ToString(), "AE601", "7a59173f-8c29-4156-96b1-50c7187e5e9e", "R5oWxaiG9YG60YT0"));
        }
        public static long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        private string EncodeToken(string timeStamp, string agentCode, string agentKey, string aesKey)
        {
            var hashToken = CreateMD5(agentCode + timeStamp + agentKey);
            var tokenPayload = $"{agentCode}|{timeStamp}|{hashToken}";
            return EncryptAES(tokenPayload, aesKey);
        }
        private string EncryptAES(string tokenPayload, string aesKey)
        {
            try
            {
                using var aes = Aes.Create();
                aes.Key = Encoding.UTF8.GetBytes(aesKey);
                aes.IV = Encoding.UTF8.GetBytes(INIT_VECTOR);
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using var encryptor = aes.CreateEncryptor();
                byte[] inputBytes = Encoding.UTF8.GetBytes(tokenPayload);
                byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error encrypting token: {ex.Message}");
                return null;
            }
        }
        private string CreateMD5(string input)
        {
            using var md5 = MD5.Create();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return Convert.ToHexString(hashBytes).ToLower();
        }
    }
    public class ApGamingBetDetail
    {
        public long WagerId { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; }
        public string ParentEventName { get; set; }
        public string HeadToHead { get; set; }
        public string WagerDateFm { get; set; }
        public string EventDateFm { get; set; }
        public string SettleDateFm { get; set; }
        public string ResettleDateFm { get; set; }
        public string Status { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public string Selection { get; set; }
        public double Handicap { get; set; }
        public double Odds { get; set; }
        public ApGamingOddTypeEnum OddsFormat { get; set; }
        public string WagerType { get; set; }
        public int BetType { get; set; }
        public long LeagueId { get; set; }
        public string League { get; set; }
        public double Stake { get; set; }
        public long SportId { get; set; }
        public string Sport { get; set; }
        public string CurrencyCode { get; set; }
        public string InplayScore { get; set; }
        public bool InPlay { get; set; }
        public string HomePitcher { get; set; }
        public string AwayPitcher { get; set; }
        public string HomePitcherName { get; set; }
        public string AwayPitcherName { get; set; }
        public long Period { get; set; }
        public string CancellationStatus { get; set; }
        public List<ParlaySelection> ParlaySelections { get; set; } = new List<ParlaySelection>();
        public string Category { get; set; }
        public double ToWin { get; set; }
        public double ToRisk { get; set; }
        public string Product { get; set; }
        public double ParlayMixOdds { get; set; }
        public double ParlayFinalOdds { get; set; }
        public string UserCode { get; set; }
        public string LoginId { get; set; }
        public double WinLoss { get; set; }
        public double Turnover { get; set; }
        public List<ScoreObject> Scores { get; set; } = new List<ScoreObject>();
        public string Result { get; set; }
        public double Volume { get; set; }
        public string View { get; set; }
    }

    public class ParlaySelection
    {
        public string Selection { get; set; }
        public string EventDateFm { get; set; }
        public List<ScoreObject> Scores { get; set; } = new List<ScoreObject>();
        public long SportId { get; set; }
        public string Sport { get; set; }
        public long LeagueId { get; set; }
        public string League { get; set; }
        public long EventId { get; set; }
        public string EventName { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public int BetType { get; set; }
        public long WagerId { get; set; }
        public string InplayScore { get; set; }
        public bool InPlay { get; set; }
        public double Odds { get; set; }
        public double Handicap { get; set; }
        public string HomePitcher { get; set; }
        public string AwayPitcher { get; set; }
        public string HomePitcherName { get; set; }
        public string AwayPitcherName { get; set; }
        public long Period { get; set; }
        public string LegStatus { get; set; }
    }

    public class ScoreObject
    {
        public int Period { get; set; }
        public string Score { get; set; }
    }

    public enum ApGamingOddTypeEnum
    {
        AmericanOdds,
        EuroOdds,
        HongKongOdds,
        IndoOdds,
        MalayOdds,
    }
}
