using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace TestConsole.Programs;

/// <summary>
/// Nagini Bet Pending API test client.
/// Calls https://nagini-api.tswltry.com/bets/pending with a transfer code and prints results.
/// </summary>
public class Nagini
{
    private static readonly HttpClient _http = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    private const string BaseUrl = "https://nagini-api.tswltry.com/bets";
    private const string BearerToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjgiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVENfVmFubmFrIiwicGVybWlzc2lvbiI6WyJCRVQuQ0FOQ0VMIiwiQkVULlNFVFRMRSIsIkJFVC5WSUVXIiwiQk9OVVMuR0VORVJBVEUiLCJCT05VUy5SRVNFVFRMRSJdLCJleHAiOjE3ODU3NTc1NTYsImlzcyI6Ik5hZ2luaVYyIiwiYXVkIjoiTmFnaW5pVUkifQ.KB3eqwfYLkves0ftTBc4YjfSM5109f3uKrmibBZgLk8";

    static Nagini()
    {
        _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {BearerToken}");
    }

    public async Task Run()
    {
        var refnos = new List<string>
        {
"PandaSports_10151_39928949_5215461421998404",
"PandaSports_10151_39928949_5215461503793123",
"PandaSports_10151_39928949_5215463502021296",
"PandaSports_10180_46249741_5250444123279786",
"PandaSports_10038_56878080_5276881787643180",
"PandaSports_10038_44366061_5276952067077832",
"PandaSports_10038_56878080_5276974053225526",
"PandaSports_10038_44441883_5277198871722972",
"PandaSports_10038_44366061_5278237838820661",
"PandaSports_10038_56762356_5278328727879582",
"PandaSports_10038_44366061_5279017438425494",
"PandaSports_10038_56878080_5279018824740768",
"PandaSports_10038_56878080_5279020053387166",
"PandaSports_10038_44441883_5280871685454251",
"PandaSports_10038_48123082_5282620294716439",
"PandaSports_10038_44366061_5282654249955387",
"PandaSports_10038_56878080_5285005990317835",
"PandaSports_10038_44389458_5288752438875814",
"PandaSports_10038_58117456_5291183779662696",
"PandaSports_10038_58117456_5291183783634812",
"PandaSports_10038_58117456_5291183785677866",
"PandaSports_10038_45880545_5301978397773106",
"PandaSports_10038_45880545_5301978507732471",
"PandaSports_10038_45992159_5302858221558714",
"PandaSports_10038_45992159_5303542652472707",
"PandaSports_10038_45992159_5303568258723854",
"PandaSports_10038_45992159_5303578685286986",
"PandaSports_10038_60137034_5303801968380970",
"PandaSports_10038_60137034_5303802778071704",
"PandaSports_10038_45992159_5306463432159779",
"PandaSports_10038_45992159_5306724374271454",
"PandaSports_10038_45992159_5306947169286851",
"PandaSports_10038_45992159_5307155351973901",
"PandaSports_10038_45992159_5307229196715007",
"PandaSports_10038_45992159_5307240759546391",
"PandaSports_10038_45992159_5307853433580809",
"PandaSports_10038_44441883_5308087812054131",
"PandaSports_10038_45992159_5308359074640779",
"PandaSports_10038_45992159_5308411757547942",
"PandaSports_10038_45992159_5308429916040154",
"PandaSports_10038_60786586_5309842796049056",
"PandaSports_10038_44441883_5309907204753358",
"PandaSports_10038_44441883_5311527408174119",
"PandaSports_10038_45992159_5311838698446816",
"PandaSports_10038_58094853_5312257112517392",
"PandaSports_10038_55302260_5312540196741187",
"PandaSports_10038_61036878_5312666777406131",
"PandaSports_10038_61036878_5312917525551491",
"PandaSports_10038_44441883_5313416709513108",
"PandaSports_10038_61341199_5314243710378634",
"PandaSports_10038_61341199_5314244181165510",
"PandaSports_10038_44480537_5314376232342320",
"PandaSports_10038_61691566_5317386785388473",
"PandaSports_10038_58013847_5318356480914181",
"PandaSports_10038_44441883_5318663014014422",
"PandaSports_10038_45992159_5319132162549321",
"PandaSports_10038_58013847_5319398215044171",
"PandaSports_10180_61598896_5319897023139859",
"PandaSports_10038_61963099_5320393870083550",
"PandaSports_10038_60970930_5320872888252550",
"PandaSports_10038_60970930_5323344428388736",
"PandaSports_10180_61598896_5323690643154663",
"PandaSports_10038_60970930_5324764292520008",
"PandaSports_10038_60970930_5325038641530681",
"PandaSports_10038_62437519_5325741280827597",
"PandaSports_10038_62437519_5325751912890479",
"PandaSports_10305_62379465_5327424047868937",
"PandaSports_10180_62719622_5327751265659602",
"PandaSports_10038_55302260_5328317150817511",
"PandaSports_10038_44998459_5328582436785003",
"PandaSports_10305_62741100_5329410477654852",
"PandaSports_10038_62927368_5329838455002582",
"PandaSports_10038_62927368_5330351671113098",
"PandaSports_10038_62927368_5330630280159576",
"PandaSports_10038_62927368_5331386696814016",
"PandaSports_10038_62927368_5331390050781621",
"PandaSports_10038_52738934_5332150018458214",
"PandaSports_10038_62993329_5332167042468056",
"PandaSports_10038_55302260_5332185749646443",
"PandaSports_10038_62927368_5332623521043578",
"PandaSports_10038_62927368_5332953694884337",
"PandaSports_10038_62927368_5333147848176112",
"PandaSports_10038_62927563_5333488403898922",
"PandaSports_10038_58419400_5333535519876014",
"PandaSports_10038_62927368_5333684642331362",
"PandaSports_10038_62993127_5333724772629677",
"PandaSports_10038_62927563_5333748385680005",
"PandaSports_10038_62927368_5333920209084433",
"PandaSports_10038_62927368_5333922686943058",
"PandaSports_10180_62719622_5333983557198542",
"PandaSports_10038_63407098_5334238981326959",
"PandaSports_10038_63407098_5334240708945979",
"PandaSports_10038_62487122_5334263284407720",
"PandaSports_10038_62980925_5334264872520902",
"PandaSports_10038_58419400_5334328653996205",
"PandaSports_10038_62993127_5334515977530122",
"PandaSports_10038_62927563_5334746151744090",
"PandaSports_10038_62286920_5335030314105991",
"PandaSports_10038_53286640_5335040972109132",
"PandaSports_10038_53286640_5335042591857974",
"PandaSports_10038_62927368_5335044631479994",
"PandaSports_10038_62980925_5335050504402273",
"PandaSports_10038_62927368_5335188201387844",
"PandaSports_10038_62927563_5335215446676956",
"PandaSports_10038_53286640_5335574767308458",
"PandaSports_10038_53286640_5335575699987703",
"PandaSports_10038_62980925_5336771270742796",
"PandaSports_10038_62980925_5336771294073650",
"PandaSports_10038_62927368_5337035967153898",
"PandaSports_10038_61552503_5337140784348544",
"PandaSports_10038_53202305_5337619707966008",
"PandaSports_10038_63896472_5337864825708328",
"PandaSports_10038_51312044_5338152741999861",
"PandaSports_10038_51312044_5338336494798434",
"PandaSports_10038_51312044_5338369639137031",
"PandaSports_10038_51312044_5338399821039395",
"PandaSports_10038_51312044_5338680443751172",
"PandaSports_10038_51312044_5338766169684721",
"PandaSports_10038_51312044_5338767347391892",
"PandaSports_10038_58248079_5338910332896722",
"PandaSports_10038_58248079_5338910448786257",
"PandaSports_10038_58248079_5339192512860303",
"PandaSports_10038_51312044_5339425829952502",
"PandaSports_10038_55327409_5340468364212149",
"PandaSports_10038_51312044_5340682739166903",
"PandaSports_10038_61552503_5340846145785853",
"PandaSports_10038_51312044_5348723678688220",
"PandaSports_10038_66140221_5348742117543951",
"PandaSports_10038_51312044_5348831961855161",
"PandaSports_10305_62741100_5349193216920000",
"PandaSports_10038_51312044_5349514495629085",
"PandaSports_10038_65228968_5349952536123788",
"PandaSports_10038_60618913_5350072682253895",
"PandaSports_10038_53645624_5350098383664916",
"PandaSports_10038_57361125_5350179176313510",
"PandaSports_10038_44761182_5350201457850045",
"PandaSports_10038_44761182_5350202017230051",
"PandaSports_10038_44761182_5350472714820218",
"PandaSports_10038_49574937_5350797707043162",
"PandaSports_10038_45992159_5351496043233075",
"PandaSports_10038_44761182_5351779617426647",
"PandaSports_10305_65553232_5352227606781022",
"PandaSports_10038_44566383_5352452508051333",
"PandaSports_10038_51312044_5352454245672879",
"PandaSports_10038_44761182_5352629353038909",
"PandaSports_10038_65061998_5352632940477080",
"PandaSports_10038_61552503_5352666447234875",
"PandaSports_10038_51312044_5352845372214338",
"PandaSports_10038_62369890_5353200687522681",
"PandaSports_10038_52738934_5353395246918227",
"PandaSports_10038_49574937_5354126968281671",
"PandaSports_10038_49574937_5354165169810909",
"PandaSports_10038_51312044_5354189360205428",
"PandaSports_10269_66971845_5354437802832573",
"PandaSports_10269_67041836_5354578225530877",
"PandaSports_10305_62741100_5354580816663646",
"PandaSports_10305_62741100_5354582746782542",
"PandaSports_10269_67041836_5354585537937111",
"PandaSports_10305_62741100_5354614811751152",
"PandaSports_10038_67904248_5354673806796961",
"PandaSports_10038_51312044_5354695703289497",
"PandaSports_10038_61552503_5354859748497963",
"PandaSports_10038_51312044_5354890591620145",
"PandaSports_10038_51312044_5354929861014302",
"PandaSports_10038_49574937_5354943453174493",
"PandaSports_10038_51312044_5354963692095578",
"PandaSports_10038_51312044_5355064928433872",
"PandaSports_10038_49574937_5355157728993195",
"PandaSports_10305_62741100_5355586056150795",
"PandaSports_10305_62571815_5355960873915237",
"PandaSports_10269_67041836_5355969353286821",
"PandaSports_10269_67041836_5355970730928580",
"PandaSports_10305_62753575_5355973155708454",
"PandaSports_10038_52738934_5356062874836226"
        };

        foreach (var refno in refnos)
        {
            Console.WriteLine($"Processing refno: {refno}");

            try
            {
                var url = $"{BaseUrl}/pending?type=NS&pageNumber=1&pageSize=20&transferCode={Uri.EscapeDataString(refno)}";
                Console.WriteLine($"  URL: {url}");

                var responseText = await _http.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<NaginiResponse>(responseText);

                if (result != null)
                {
                    Console.WriteLine($"  Success   : {result.Success}");
                    Console.WriteLine($"  Message   : {result.Message}");
                    Console.WriteLine($"  Total     : {result.Data.TotalCount} ({result.Data.PageSize} items/page, page {result.Data.PageNumber}/{result.Data.TotalPages})");
                    Console.WriteLine($"  Items     : {result.Data.Items.Count}");
                    Console.WriteLine();

                    foreach (var item in result.Data.Items)
                    {
                        PrintBetItem(item);
                        await SettleBetAsLost(item);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error: {ex.Message}");
            }

            Console.WriteLine($"Finished processing refno: {refno} =================");
        }
    }

    private async Task SettleBetAsLost(NaginiBetItem item)
    {
        var settleRequest = new NaginiSettleRequest
        {
            Type = "NS",
            GpId = item.ProviderId,
            AccountId = item.CustomerId,
            DealId = item.TransactionId,
            Refno = item.TransferCode,
            Stake = item.Stake,
            WinLost = 0,
            CommissionStake = item.CommissionStake,
            GameResult = "",
            OrderDetail = item.OrderDetail,
            ExtraInfo = item.ExtraInfo,
            Currency = item.Currency,
            ProviderType = item.ProviderType
        };
        // call this post api /bets/settle with this request NaginiSettleRequest
        var settleUrl = $"{BaseUrl}/settle";
        Console.WriteLine($"  Settling: {settleUrl}");

        var json = JsonConvert.SerializeObject(settleRequest);
        Console.WriteLine($"  Request: {json}");

        try
        {
            var content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
            var settleResponse = await _http.PostAsync(settleUrl, content);
            var settleBody = await settleResponse.Content.ReadAsStringAsync();

            Console.WriteLine($"  Status: {(int)settleResponse.StatusCode} {settleResponse.StatusCode}");
            Console.WriteLine($"  Response: {settleBody}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Settle error: {ex.Message}");
        }
    }

    /// <summary>
    /// Print a single bet item in a readable format.
    /// </summary>
    private static void PrintBetItem(NaginiBetItem item)
    {
        Console.WriteLine($"  ┌─────────────────────────────────────────────┐");
        Console.WriteLine($"  │ Provider  : {item.Provider,-40}│");
        Console.WriteLine($"  │ Transfer  : {item.TransferCode,-40}│");
        Console.WriteLine($"  │ RoundId   : {item.GameRoundId,-40}│");
        Console.WriteLine($"  │ TransId   : {item.TransactionId,-40}│");
        Console.WriteLine($"  │ Customer  : {item.CustomerId,-40}│");
        Console.WriteLine($"  │ Currency  : {item.Currency,-40}│");
        Console.WriteLine($"  │ Stake     : {item.Stake?.ToString() ?? "null",-40}│");
        Console.WriteLine($"  │ Result    : {item.GameResult,-40}│");
        Console.WriteLine($"  │ OrderOn   : {(item.OrderOn?.ToString("yyyy-MM-dd HH:mm:ss") ?? "null"),-38}│");
        Console.WriteLine($"  └─────────────────────────────────────────────┘");
    }
}


// ── Nagini API response models ─────────────────────────────────────────────

/// <summary>
/// Outer wrapper for the Nagini API response.
/// { "success": bool, "message": string, "data": Data }
/// </summary>
public class NaginiResponse
{
    /// <summary>Whether the request succeeded</summary>
    [JsonProperty("success")]
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>Status message from the API</summary>
    [JsonProperty("message")]
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>Pagination payload containing the bet items</summary>
    [JsonProperty("data")]
    [JsonPropertyName("data")]
    public NaginiData Data { get; set; } = new();
}

/// <summary>
/// Paginated data container returned by the Nagini API.
/// </summary>
public class NaginiData
{
    /// <summary>List of bet / transaction items</summary>
    [JsonProperty("items")]
    [JsonPropertyName("items")]
    public List<NaginiBetItem> Items { get; set; } = new();

    /// <summary>Total number of records across all pages</summary>
    [JsonProperty("totalCount")]
    [JsonPropertyName("totalCount")]
    public long? TotalCount { get; set; }

    /// <summary>Current page number (1-based)</summary>
    [JsonProperty("pageNumber")]
    [JsonPropertyName("pageNumber")]
    public long? PageNumber { get; set; }

    /// <summary>Number of items per page</summary>
    [JsonProperty("pageSize")]
    [JsonPropertyName("pageSize")]
    public long? PageSize { get; set; }

    /// <summary>Total number of pages</summary>
    [JsonProperty("totalPages")]
    [JsonPropertyName("totalPages")]
    public long? TotalPages { get; set; }
}

/// <summary>
/// A single bet / transaction record from the Nagini API.
/// </summary>
public class NaginiBetItem
{
    /// <summary>Game provider name (e.g. "NetEnt")</summary>
    [JsonProperty("provider")]
    [JsonPropertyName("provider")]
    public string Provider { get; set; } = string.Empty;

    /// <summary>Game provider type</summary>
    [JsonProperty("providerType")]
    [JsonPropertyName("providerType")]
    public string ProviderType { get; set; } = string.Empty;

    /// <summary>Internal provider identifier</summary>
    [JsonProperty("providerId")]
    [JsonPropertyName("providerId")]
    public long? ProviderId { get; set; }

    /// <summary>Web-facing provider ID</summary>
    [JsonProperty("webId")]
    [JsonPropertyName("webId")]
    public long? WebId { get; set; }

    /// <summary>Deal / campaign identifier</summary>
    [JsonProperty("dealId")]
    [JsonPropertyName("dealId")]
    public string DealId { get; set; } = string.Empty;

    /// <summary>Unique transfer code (used as query param)</summary>
    [JsonProperty("transferCode")]
    [JsonPropertyName("transferCode")]
    public string TransferCode { get; set; } = string.Empty;

    /// <summary>Amount staked</summary>
    [JsonProperty("stake")]
    [JsonPropertyName("stake")]
    public long? Stake { get; set; }

    /// <summary>Transaction identifier</summary>
    [JsonProperty("transactionId")]
    [JsonPropertyName("transactionId")]
    public string TransactionId { get; set; } = string.Empty;

    /// <summary>Game period identifier</summary>
    [JsonProperty("gamePeriodId")]
    [JsonPropertyName("gamePeriodId")]
    public string GamePeriodId { get; set; } = string.Empty;

    /// <summary>Game round identifier</summary>
    [JsonProperty("gameRoundId")]
    [JsonPropertyName("gameRoundId")]
    public string GameRoundId { get; set; } = string.Empty;

    /// <summary>Customer identifier</summary>
    [JsonProperty("customerId")]
    [JsonPropertyName("customerId")]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>Agent identifier (if applicable)</summary>
    [JsonProperty("agentId")]
    [JsonPropertyName("agentId")]
    public string AgentId { get; set; } = string.Empty;

    /// <summary>Currency code (e.g. "USD", "MYR")</summary>
    [JsonProperty("currency")]
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>Timestamp when the order was placed</summary>
    [JsonProperty("orderOn")]
    [JsonPropertyName("orderOn")]
    public DateTime? OrderOn { get; set; }

    /// <summary>Total count associated with this record</summary>
    [JsonProperty("totalCount")]
    [JsonPropertyName("totalCount")]
    public long? TotalCount { get; set; }

    /// <summary>Commission stake amount</summary>
    [JsonProperty("commissionStake")]
    [JsonPropertyName("commissionStake")]
    public long? CommissionStake { get; set; }

    /// <summary>Result of the game round (e.g. "Win", "Loss")</summary>
    [JsonProperty("gameResult")]
    [JsonPropertyName("gameResult")]
    public string GameResult { get; set; } = string.Empty;

    /// <summary>Detailed order information</summary>
    [JsonProperty("orderDetail")]
    [JsonPropertyName("orderDetail")]
    public string OrderDetail { get; set; } = string.Empty;

    /// <summary>Additional metadata</summary>
    [JsonProperty("extraInfo")]
    [JsonPropertyName("extraInfo")]
    public string ExtraInfo { get; set; } = string.Empty;
}

public class NaginiSettleRequest
{
    /// <summary>Bet type (e.g. "NS" for Netsport)</summary>
    [JsonProperty("type")]
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Game provider ID</summary>
    [JsonProperty("gpId")]
    [JsonPropertyName("gpId")]
    public long? GpId { get; set; }

    /// <summary>Player account identifier</summary>
    [JsonProperty("accountId")]
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; } = string.Empty;

    /// <summary>Deal / campaign identifier</summary>
    [JsonProperty("dealId")]
    [JsonPropertyName("dealId")]
    public string DealId { get; set; } = string.Empty;

    /// <summary>Reference number / transfer code</summary>
    [JsonProperty("refno")]
    [JsonPropertyName("refno")]
    public string Refno { get; set; } = string.Empty;

    /// <summary>Amount staked</summary>
    [JsonProperty("stake")]
    [JsonPropertyName("stake")]
    public long? Stake { get; set; }

    /// <summary>Win/loss amount (positive=win, negative=loss)</summary>
    [JsonProperty("winlost")]
    [JsonPropertyName("winlost")]
    public long? WinLost { get; set; }

    /// <summary>Commission stake amount</summary>
    [JsonProperty("commissionStake")]
    [JsonPropertyName("commissionStake")]
    public long? CommissionStake { get; set; }

    /// <summary>Game result (e.g. "Win", "Loss")</summary>
    [JsonProperty("gameResult")]
    [JsonPropertyName("gameResult")]
    public string GameResult { get; set; } = string.Empty;

    /// <summary>Detailed order information</summary>
    [JsonProperty("orderDetail")]
    [JsonPropertyName("orderDetail")]
    public string OrderDetail { get; set; } = string.Empty;

    /// <summary>Additional metadata</summary>
    [JsonProperty("extraInfo")]
    [JsonPropertyName("extraInfo")]
    public string ExtraInfo { get; set; } = string.Empty;

    /// <summary>Currency code (e.g. "USD", "MYR")</summary>
    [JsonProperty("currency")]
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>Provider type (e.g. "NAGINI")</summary>
    [JsonProperty("providerType")]
    [JsonPropertyName("providerType")]
    public string ProviderType { get; set; } = string.Empty;
}