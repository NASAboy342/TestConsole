using System;
using System.Web;
using Newtonsoft.Json;

namespace TestConsole.Programs;

public class cq9
{
    // ── INSERT YOUR VALUES HERE ──────────────────────────────────────────────
    private static readonly string ApiUrl        = "https://apie.cqgame.cc/gameboy/order/view"; // e.g. "https://api.cqnine.com/v1/betlist"
    private static readonly string Authorization = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyaWQiOiI2MTY2NmRjYWZlMmJmZTAwMDE1NGQ2YWEiLCJhY2NvdW50IjoiNjFTQk9USEIiLCJvd25lciI6IjVjOWMyMGYzYjU3YmM4MDAwMTk2YjVjOSIsInBhcmVudCI6IjVjOWMyMGYzYjU3YmM4MDAwMTk2YjVjOSIsImN1cnJlbmN5IjoiVEhCIiwiYnJhbmQiOiJjcTkiLCJqdGkiOiI4Njk0ODkzOTEiLCJpYXQiOjE3ODI0NjMzNjAsImlzcyI6IkN5cHJlc3MiLCJzdWIiOiJTU1Rva2VuIn0.QzgZ3mlKqc7Ts0jcoJVCBo_SJs7cCxVc44eFdOKk1nI"; // vendor signature / Bearer token
    private static readonly DateTime StartTime   = DateTime.UtcNow.AddDays(-3); // e.g. DateTime.UtcNow.AddDays(-3) for 3 days ago
    private static readonly DateTime EndTime     = DateTime.UtcNow;
    private static readonly int PageSize         = 100;
    // ────────────────────────────────────────────────────────────────────────

    public async Task Run()
    {
        var result = new List<BetData>();
        for (var date = StartTime.Date; date <= EndTime.Date; date = date.AddDays(1))
        {
            var dayStart = date;
            var dayEnd   = date.AddDays(1).AddTicks(-1);
            var dailyResult = await GetBetListAsync(dayStart, dayEnd, ApiUrl, Authorization);
            result.AddRange(dailyResult);
            Console.WriteLine($"Fetched {dailyResult.Count} records for {date:yyyy-MM-dd}");
            await Task.Delay(1000); // Optional: Delay between requests to avoid rate limiting
        }

        Console.WriteLine($"Total records fetched: {result.Count}");
        // replace content into this file "/Users/pinsopheaktra/Downloads/cq9fishgame.json"
        System.IO.File.WriteAllText("/Users/pinsopheaktra/Downloads/cq9fishgame.json", JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    private static async Task<List<BetData>> GetBetListAsync(
        DateTime startTime,
        DateTime endTime,
        string apiUrl,
        string authorization)
    {
        var result = new List<BetData>();
        var page = 1;
        var isNextPageExist = true;

        using var client = new HttpClient();

        while (isNextPageExist)
        {
            try
            {
                var request = new GetBetListRequest
                {
                    StartDate = new DateTimeOffset(startTime, TimeSpan.Zero).ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    EndDate   = new DateTimeOffset(endTime,   TimeSpan.Zero).ToOffset(TimeSpan.FromHours(-4)).ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    Page      = page,
                    PageSize  = PageSize,
                };

                var query = BuildQueryString(request);
                var url = $"{apiUrl}?{query}";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, url);
                httpRequest.Headers.TryAddWithoutValidation("Authorization", authorization);

                var httpResponse = await client.SendAsync(httpRequest);
                var stringResponse = await httpResponse.Content.ReadAsStringAsync();

                Console.WriteLine($"[Page {page}] Status: {httpResponse.StatusCode}");

                var providerResponse = JsonConvert.DeserializeObject<GetBetListResponse>(stringResponse);

                if (providerResponse?.Status?.Code == 0 && providerResponse.Result?.Data?.Count > 0)
                {
                    var fishgameData = providerResponse.Result.Data.Where(bet => bet.GameType.Equals("vpfish", StringComparison.OrdinalIgnoreCase));
                    Console.WriteLine(JsonConvert.SerializeObject(fishgameData));
                    result.AddRange(fishgameData);

                    var totalPage = (providerResponse.Result.TotalSize / request.PageSize) + 1;

                    if (page >= totalPage)
                        isNextPageExist = false;

                    page++;
                }
                else
                {
                    Console.WriteLine($"[Page {page}] No more data or non-zero status code. Raw: {stringResponse}");
                    isNextPageExist = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Page {page}] Exception: {ex.Message}");
                isNextPageExist = false;
            }
        }

        return result;
    }

    private static string BuildQueryString(GetBetListRequest request)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["starttime"] = request.StartDate;
        query["endtime"]   = request.EndDate;
        query["page"]      = request.Page.ToString();
        query["pagesize"]  = request.PageSize.ToString();
        return query.ToString();
    }
}

// ── Models ────────────────────────────────────────────────────────────────────

public class GetBetListRequest
{
    public string StartDate { get; set; }
    public string EndDate   { get; set; }
    public int    Page      { get; set; }
    public int    PageSize  { get; set; } = 5000;
}

public class GetBetListResponse
{
    [JsonProperty("data")]
    public ResultData Result { get; set; }

    [JsonProperty("status")]
    public ResponseStatus Status { get; set; }
}

public class ResponseStatus
{
    [JsonProperty("code")]
    public int Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("traceCode")]
    public string TraceCode { get; set; }

    [JsonProperty("dateTime")]
    public DateTime DateTime { get; set; }
}

public class ResultData
{
    [JsonProperty("TotalSize")]
    public int TotalSize { get; set; }

    [JsonProperty("Data")]
    public List<BetData> Data { get; set; } = new();
}

public class BetData
{
    [JsonProperty("gamehall")]    public string GameHall  { get; set; }
    [JsonProperty("gametype")]    public string GameType  { get; set; }
    [JsonProperty("gameplat")]    public string GamePlat  { get; set; }
    [JsonProperty("gamecode")]    public string GameCode  { get; set; }
    [JsonProperty("account")]     public string Account   { get; set; }
    [JsonProperty("round")]       public string Round     { get; set; }
    [JsonProperty("balance")]     public decimal Balance  { get; set; }
    [JsonProperty("win")]         public decimal Win      { get; set; }
    [JsonProperty("bet")]         public decimal Bet      { get; set; }
    [JsonProperty("validbet")]    public decimal ValidBet { get; set; }
    [JsonProperty("jackpot")]     public decimal Jackpot  { get; set; }
    [JsonProperty("status")]      public string Status    { get; set; }
    [JsonProperty("endroundtime")] public DateTime EndRoundTime { get; set; }
    [JsonProperty("createtime")]  public DateTime CreateTime   { get; set; }
    [JsonProperty("bettime")]     public DateTime BetTime      { get; set; }
    [JsonProperty("singlerowbet")] public bool SingleRowBet   { get; set; }
    [JsonProperty("gamerole")]    public string GameRole  { get; set; }
    [JsonProperty("bankertype")]  public string BankerType { get; set; }
    [JsonProperty("rake")]        public decimal Rake     { get; set; }
    [JsonProperty("roomfee")]     public decimal RoomFee  { get; set; }
    [JsonProperty("tabletype")]   public string TableType { get; set; }
    [JsonProperty("tableid")]     public string TableId   { get; set; }
    [JsonProperty("roundnumber")] public string RoundNumber { get; set; }
    [JsonProperty("currency")]    public string Currency  { get; set; }
    [JsonProperty("bettype")]     public List<string> BetType     { get; set; }
    [JsonProperty("gameresult")]  public GameResult GameResult    { get; set; }
    [JsonProperty("detail")]      public List<BetDetail> Detail  { get; set; } = new();
}

public class BetDetail
{
    [JsonProperty("freegame")]  public decimal  FreeGame  { get; set; }
    [JsonProperty("luckydraw")] public decimal? LuckyDraw { get; set; }
    [JsonProperty("bonus")]     public decimal? Bonus     { get; set; }
}

public class GameResult
{
    [JsonProperty("points")] public List<decimal> Points { get; set; } = new();
    [JsonProperty("cards")]  public List<Card>    Cards  { get; set; } = new();
}

public class Card
{
    [JsonProperty("poker")] public string Poker { get; set; }
    [JsonProperty("tag")]   public int    Tag   { get; set; }
}

