using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestConsole.Programs;

public class SabaRequestGenerater
{
    // private const string BaseUrl  = "https://sapi-uat.xxttgg.com";
    private const string BaseUrl  = "https://sapi-a13.568winex.com";
    private const string ApiKey   = "1vdw3fnhu3";
    private const int    Currency = 20;

    private static readonly HttpClient        _http    = new();
    private static readonly Random            _rng     = new();
    private static readonly JsonSerializerOptions _json = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        WriteIndented           = false
    };

    // ── ID generators ─────────────────────────────────────────────────────────
    private static long GenTxId() =>
        long.Parse(
            $"{_rng.Next(1, 10)}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10000):D4}" +
            $"{_rng.Next(0, 10):D1}");

    private static int GenPrefix()    => _rng.Next(1000000, 9999999);
    private static int GenBetId()     => _rng.Next(1000, 9999);
    private static int GenOpCounter() => _rng.Next(10000, 99999);

    // ── Input helper ──────────────────────────────────────────────────────────
    private static (string userId, string betAmount, string payout) PromptInput()
    {
        Console.Clear();
        Console.WriteLine("┌─────────────────────────────────────┐");
        Console.WriteLine("│     Saba Request Generator          │");
        Console.WriteLine("└─────────────────────────────────────┘");
        Console.Write("  User ID    : ");
        string userId = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID is required.");

        Console.Write("  Bet amount : ");
        string betAmount = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(betAmount)) throw new ArgumentException("Bet amount is required.");

        Console.Write("  Payout     : ");
        string payout = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(payout)) throw new ArgumentException("Payout is required.");

        return (userId, betAmount, payout);
    }

    // ── IDs record ────────────────────────────────────────────────────────────
    private record Ids(
        string UserId, string BetAmount, string Payout,
        int Prefix, int BetId, long TxId, int OpBase);

    private static Ids GenerateIds(string userId, string betAmount, string payout) =>
        new(userId, betAmount, payout,
            GenPrefix(), GenBetId(), GenTxId(), GenOpCounter());

    // ── Menu ──────────────────────────────────────────────────────────────────
    public async Task Run()
    {
        var (userId, betAmount, payout) = PromptInput();
        Ids ids = GenerateIds(userId, betAmount, payout);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────────────────────┐");
            Console.WriteLine($"│  User: {ids.UserId,-20} Bet: {ids.BetAmount,-8} Payout: {ids.Payout,-8} │");
            Console.WriteLine($"│  Prefix: {ids.Prefix}  BetId: {ids.BetId}  TxId: {ids.TxId} │");
            Console.WriteLine("├───────────────────────────────────────────────────────┤");
            Console.WriteLine("│  [1]  PlaceBet                                        │");
            Console.WriteLine("│  [2]  ConfirmBet                                      │");
            Console.WriteLine("│  [3]  Settle (won)                                    │");
            Console.WriteLine("│  [4]  Settle (lost)                                   │");
            Console.WriteLine("│  [A]  All — PlaceBet → ConfirmBet → Settle (won)      │");
            Console.WriteLine("│  [R]  Re-generate IDs                                 │");
            Console.WriteLine("│  [I]  Change inputs                                   │");
            Console.WriteLine("│  [Q]  Quit                                            │");
            Console.WriteLine("└───────────────────────────────────────────────────────┘");
            Console.Write("  Choice: ");

            string choice = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";
            Console.WriteLine();

            switch (choice)
            {
                case "1": await CallPlaceBet(ids);        break;
                case "2": await CallConfirmBet(ids);      break;
                case "3": await CallSettle(ids, "won");   break;
                case "4": await CallSettle(ids, "lost");  break;
                case "A":
                    await CallPlaceBet(ids);
                    await CallConfirmBet(ids);
                    await CallSettle(ids, "won");
                    break;
                case "R":
                    ids = GenerateIds(ids.UserId, ids.BetAmount, ids.Payout);
                    Console.WriteLine("  New IDs generated.");
                    break;
                case "I":
                    (userId, betAmount, payout) = PromptInput();
                    ids = GenerateIds(userId, betAmount, payout);
                    break;
                case "Q":
                    return;
                default:
                    Console.WriteLine("  Invalid choice.");
                    break;
            }

            if (choice != "I")
            {
                Console.WriteLine();
                Console.Write("  Press any key to return to menu...");
                Console.ReadKey(intercept: true);
            }
        }
    }

    // ── Call methods ──────────────────────────────────────────────────────────
    private static async Task CallPlaceBet(Ids i)
    {
        string refId       = $"{i.Prefix}_{i.BetId}_U";
        string operationId = $"{i.Prefix}_1_{i.OpBase}_U";
        double betAmt      = double.Parse(i.BetAmount);

        var body = new
        {
            key     = ApiKey,
            message = new
            {
                userId       = i.UserId,
                currency     = Currency,
                betAmount    = betAmt,
                actualAmount = 0.0,
                debitAmount  = 0.0,
                refId,
                sportType    = 1,
                odds         = 0.95,
                oddsType     = 1,
                betType      = 1,
                betTeam      = "h",
                voucher      = new[] { new { type = 5, voucherId = 1133, quota = betAmt } },
                action       = "PlaceBet",
                operationId
            }
        };

        Console.WriteLine($"  refId       : {refId}");
        Console.WriteLine($"  operationId : {operationId}");
        await SendAsync("PlaceBet", $"{BaseUrl}/api/sabaseamlesswallet/placebet", body);
    }

    private static async Task CallConfirmBet(Ids i)
    {
        string licenseeTxId = $"{i.Prefix}_{i.BetId}_U";
        string operationId  = $"{i.Prefix}_2_{i.OpBase + 1}_U";
        string updateTime   = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");

        var body = new
        {
            key     = ApiKey,
            message = new
            {
                userId = i.UserId,
                updateTime,
                txns = new[]
                {
                    new
                    {
                        txId          = i.TxId,
                        licenseeTxId,
                        odds          = 0.95,
                        oddsType      = 1,
                        actualAmount  = 0.0,
                        isOddsChanged = false
                    }
                },
                action      = "ConfirmBet",
                operationId
            }
        };

        Console.WriteLine($"  licenseeTxId : {licenseeTxId}");
        Console.WriteLine($"  txId         : {i.TxId}");
        Console.WriteLine($"  operationId  : {operationId}");
        await SendAsync("ConfirmBet", $"{BaseUrl}/api/sabaseamlesswallet/confirmbet", body);
    }

    private static async Task CallSettle(Ids i, string status)
    {
        string operationId = $"{i.Prefix}_4_{i.OpBase + 2}_U";
        string updateTime  = DateTimeOffset.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
        string winlostDate = new DateTimeOffset(DateTimeOffset.Now.Date, DateTimeOffset.Now.Offset)
                                 .ToString("yyyy-MM-ddTHH:mm:sszzz");
        double payout      = status == "won" ? double.Parse(i.Payout) : 0.0;

        var body = new
        {
            key     = ApiKey,
            message = new
            {
                txns = new[]
                {
                    new
                    {
                        userId = i.UserId,
                        txId   = i.TxId,
                        updateTime,
                        winlostDate,
                        status,
                        payout
                    }
                },
                action      = "Settle",
                operationId
            }
        };

        Console.WriteLine($"  txId        : {i.TxId}");
        Console.WriteLine($"  status      : {status}");
        Console.WriteLine($"  payout      : {payout}");
        Console.WriteLine($"  operationId : {operationId}");
        await SendAsync($"Settle ({status})", $"{BaseUrl}/api/sabaseamlesswallet/settle", body);
    }

    // ── HTTP helper ───────────────────────────────────────────────────────────
    private static async Task SendAsync(string label, string url, object body)
    {
        Console.WriteLine($"  Calling {label}...");
        try
        {
            string json     = JsonSerializer.Serialize(body, _json);
            var    content  = new StringContent(json, Encoding.UTF8, "application/json");
            var    response = await _http.PostAsync(url, content);
            string respBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"  Status   : {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"  Request  : {json}");
            Console.WriteLine($"  Response : {respBody}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }
    }
}
