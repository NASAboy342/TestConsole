using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace TestConsole.Programs;

/// <summary>
/// Pragmatic Play Casino Integration API — test client.
/// Implements hash calculation and round-status checking per the API spec.
/// </summary>
public class PragmaticPlayApi
{
    private const string BaseUrl = "https://api-sg57-gp.ppgames.net/IntegrationService/v3/http/HistoryAPI";
    private const string SecretKey = "c4F9d1_75fA@6aDd2";
    private const string SecureLogin = "sbo_sbo";

    private static readonly HttpClient _http = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    private static readonly JsonSerializerOptions _jsonWrite = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        WriteIndented = false
    };

    private static readonly JsonSerializerOptions _jsonRead = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // ── Menu ──────────────────────────────────────────────────────────────────
    public async Task Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("┌───────────────────────────────────────────────────────────┐");
            Console.WriteLine("│  Pragmatic Play Casino API (HistoryAPI)                   │");
            Console.WriteLine("├───────────────────────────────────────────────────────────┤");
            Console.WriteLine("│  [1]  GetRoundStatus                                      │");
            Console.WriteLine("│  [2]  Test hash calculation                               │");
            Console.WriteLine("│  [Q]  Quit                                                │");
            Console.WriteLine("└───────────────────────────────────────────────────────────┘");
            Console.Write("  Choice: ");

            string choice = Console.ReadLine()?.Trim().ToUpperInvariant() ?? "";

            switch (choice)
            {
                case "1": await CallGetRoundStatus(); break;
                case "2": TestHashCalculation();    break;
                case "Q": return;
                default:  Console.WriteLine("  Invalid choice."); break;
            }

            if (choice != "Q" && choice != "1" && choice != "2")
            {
                Console.Write("  Press any key to return to menu...");
                Console.ReadKey(intercept: true);
            }
        }
    }

    // ── GetRoundStatus ────────────────────────────────────────────────────────
    /// <summary>
    /// POST /IntegrationService/v3/http/HistoryAPI/GetRoundStatus/
    /// Returns the current status of a particular game round.
    /// </summary>
    private static async Task CallGetRoundStatus()
    {
        Console.Clear();
        Console.WriteLine("┌───────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  GetRoundStatus                                           │");
        Console.WriteLine("└───────────────────────────────────────────────────────────┘");

        // Console.Write("  Secure Login : ");
        string secureLogin = SecureLogin;

        Console.Write("  Round ID     : ");
        string roundId = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(roundId))
        {
            Console.WriteLine("  Round ID is required.");
            Console.Write("  Press any key to return to menu...");
            Console.ReadKey(intercept: true);
            return;
        }

        Console.Write("  Game ID      : ");
        string gameId = Console.ReadLine()?.Trim() ?? "";
        if (string.IsNullOrEmpty(gameId)) gameId = "vs50aladdin";

        Console.Write("  Options      : (comma-separated, e.g. partialWinStatus,addCurrency)");
        string optionsRaw = Console.ReadLine()?.Trim() ?? "";

        string url = $"{BaseUrl}/GetRoundStatus/";

        // Build form-encoded parameters (without hash first)
        var formParams = new List<KeyValuePair<string, string>>
        {
            new("secureLogin", secureLogin),
            new("roundId", roundId),
            new("gameId", gameId)
        };

        // Optional options parameter — combine into a single comma-separated value
        if (!string.IsNullOrEmpty(optionsRaw))
        {
            formParams.Add(new("options", optionsRaw));
        }

        // Calculate hash from sorted parameters (excluding hash itself)
        string hash = CalculateHash(formParams);
        formParams.Add(new("hash", hash));

        Console.WriteLine();
        Console.WriteLine($"  Base URL : {url}");
        Console.WriteLine($"  Params   : {string.Join(" & ", formParams.Select(p => $"{p.Key}={p.Value}"))}");
        Console.WriteLine($"  Hash     : {hash}");
        Console.WriteLine();
        Console.WriteLine("  Calling...");

        try
        {
            var content = new FormUrlEncodedContent(formParams);
            var response = await _http.PostAsync(url, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"  Status: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"  Response: {responseBody}");

            // Pretty-print the JSON response
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                Console.WriteLine($"  Pretty: {System.Text.Json.JsonSerializer.Serialize(doc.RootElement, _jsonWrite)}");

                var result = System.Text.Json.JsonSerializer.Deserialize<GetRoundStatusResponse>(responseBody, _jsonRead);
                if (result != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("  Parsed response:");
                    Console.WriteLine($"    Error       : {result.Error}");
                    Console.WriteLine($"    Description : {result.Description}");
                    if (result.RoundId.HasValue)
                        Console.WriteLine($"    RoundId     : {result.RoundId}");
                    if (result.BetAmount != null)
                        Console.WriteLine($"    BetAmount   : {result.BetAmount}");
                    if (result.WinAmount != null)
                        Console.WriteLine($"    WinAmount   : {result.WinAmount}");
                    if (result.RoundStatus != null)
                        Console.WriteLine($"    RoundStatus : {result.RoundStatus}");
                    if (result.Currency != null)
                        Console.WriteLine($"    Currency    : {result.Currency}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  (Could not parse JSON response: {ex.Message})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }

        Console.Write("  Press any key to return to menu...");
        Console.ReadKey(intercept: true);
    }

    // ── Hash calculation ─────────────────────────────────────────────────────
    /// <summary>
    /// Hash code is calculated with the following formula:
    /// 1. Sort all parameters by key in alphabetical order.
    /// 2. Append them (if the value is not null or empty) as key1=value1&key2=value2.
    /// 3. Append the secret key: ...SECRET.
    /// 4. Calculate MD5 hash.
    /// 5. Return lowercase hex string.
    /// </summary>
    private static string CalculateHash(List<KeyValuePair<string, string>> parameters)
    {
        // Step 1: sort by key alphabetically
        var sorted = parameters
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .ToList();

        // Step 2 & 3: build the canonical string (skip null/empty values)
        var sb = new StringBuilder();
        foreach (var param in sorted)
        {
            if (!string.IsNullOrEmpty(param.Value))
            {
                if (sb.Length > 0) sb.Append('&');
                sb.Append(param.Key).Append('=').Append(param.Value);
            }
        }
        sb.Append(SecretKey);

        string canonical = sb.ToString();

        // Debug: show what we're hashing
        Console.WriteLine($"  Canonical string: {canonical}");

        // Step 4: MD5 hash
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(canonical));

        // Step 5: return as lowercase hex
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    private static void TestHashCalculation()
    {
        Console.Clear();
        Console.WriteLine("┌───────────────────────────────────────────────────────────┐");
        Console.WriteLine("│  Hash Calculation Test                                    │");
        Console.WriteLine("└───────────────────────────────────────────────────────────┘");

        // Test with example from API spec:
        // secureLogin=username&roundId=5108924498&gameId=vs50aladdin&currency=USD
        // Hash expected: 8567449f06333293030e8f80ec89d3fa (from API docs example)

        Console.WriteLine("  Test case from API spec:");
        var testParams = new List<KeyValuePair<string, string>>
        {
            new("secureLogin", "username"),
            new("roundId", "5108924498"),
            new("gameId", "vs50aladdin"),
            new("currency", "USD")
        };

        string hash = CalculateHash(testParams);
        Console.WriteLine($"  Calculated hash: {hash}");

        // The example hash from the docs includes the secret key appended
        // If we use a placeholder secret, the hash won't match — that's expected
        Console.WriteLine("  (Hash will differ until SecretKey is configured correctly)");

        Console.Write("  Press any key to return to menu...");
        Console.ReadKey(intercept: true);
    }
}

// ── Response model ──────────────────────────────────────────────────────────

/// <summary>
/// Response model for GetRoundStatus API call.
/// </summary>
public class GetRoundStatusResponse
{
    /// <summary>Error code (0 = success)</summary>
    [JsonProperty("error")]
    [JsonPropertyName("error")]
    public string Error { get; set; } = string.Empty;

    /// <summary>Error description for troubleshooting</summary>
    [JsonProperty("description")]
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Id of the game round</summary>
    [JsonProperty("roundId")]
    [JsonPropertyName("roundId")]
    public long? RoundId { get; set; }

    /// <summary>Amount of the bet</summary>
    [JsonProperty("betAmount")]
    [JsonPropertyName("betAmount")]
    public string? BetAmount { get; set; }

    /// <summary>Amount of the winnings</summary>
    [JsonProperty("winAmount")]
    [JsonPropertyName("winAmount")]
    public string? WinAmount { get; set; }

    /// <summary>Status of the game round.</summary>
    /// <summary>Possible values: In progress, Completed, Canceled, CompleteInProcess, CancelInProcess, Partial win</summary>
    [JsonProperty("roundStatus")]
    [JsonPropertyName("roundStatus")]
    public string? RoundStatus { get; set; }

    /// <summary>Currency of the transaction (3-letter ISO code). Optional.</summary>
    [JsonProperty("currency")]
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}