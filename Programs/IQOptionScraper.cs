using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace TestConsole.Programs;

// ═══════════════════════════════════════════════════════════════════════════════
// Scraper
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Connects to the IQ Option PWA WebSocket (wss://ws.km.iqoption.com/echo/websocket)
/// and reads candle data. Full protocol verified live 2026-08-19:
///
///   1. Connect — NO token in URL. "identity" cookie in the handshake header is
///      OPTIONAL (server authenticates with just the ssid below).
///   2. Send {"name":"authenticate","msg":{"ssid":"...","protocol":3}} →
///      server replies {"name":"authenticated","msg":true}.
///   3. Historical: {"name":"sendMessage","msg":{"name":"quotes-history.get-first-candles",
///      "version":"1.0","body":{"active_id":1912}}}
///      → replies {"request_id":"...","name":"candles","msg":{"candles":[...]}}.
///      NOTE: the old "quotes-history.get-candles" (v2.0) needs a real from_id/tail id
///      and returns empty when given 0 — the web client uses get-first-candles instead.
///   4. Live stream: {"name":"subscribeMessage","msg":{"name":"quotes.candle-generated",
///      "params":{"routingFilters":{"active_id":1912,"size":5}},"version":"1.0"}}
///      → server pushes {"name":"candle-generated","msg":{"active_id":...,...}} per tick.
///   5. Keep-alive: {"resource":"ping","timestamp":ms} every ~5s.
///
/// ActiveId 1912 = Gold (spot ~4493), 1861 = EUR/USD (observed in live traffic).
/// Paste your live "ssid" cookie (DevTools → Application → Cookies on km.iqoption.com)
/// into Ssid. It rotates when the session changes.
/// </summary>
public class IQOptionScraper
{
    // ── Configuration ──────────────────────────────────────────────────────────
    private const string WebSocketUrl = "wss://ws.km.iqoption.com/echo/websocket";

    /// <summary>
    /// The "ssid" cookie from km.iqoption.com (DevTools → Application → Cookies).
    /// Used in the post-connect "authenticate" frame.
    /// </summary>
    private const string Ssid = "24c1a7c8e53fe13e1ea825be5248f9ch";

    /// <summary>
    /// OPTIONAL. The "identity" cookie from km.iqoption.com, sent in the WS handshake
    /// Cookie header. Verified 2026-08-19: authentication succeeds with only the ssid,
    /// so this can be left empty. Kept here in case the server tightens its checks.
    /// </summary>
    private const string IdentityCookie = "901b317a53bee6be26e4437608f80e9fc28555193d58c6a1b74b07f8385825e36c2c04963c5bbb7ba7003e00e13ba20c08a3777689e69fb16127f8c5e576b3c6536d2fa1ae20d0200831ff4e3aad6d9c9f8a141cd0d017289fb9ab0a30a253a4272da0bd9115d8e9e3d2689c8d862bebae7bbfa37af986cb19cf02bd11b27500f32ab591465630e11056c1ceab425c2fb8776bff3f8619a6c5ff06d1e9fc06bb4d3619e62c31c6b6e6be07e14bce9d11db07b45f8dfe3206e6be07e14bce9d11fecfc8cc5ec99f427e58a3757ce1eb443d755b4816b7b5da"; // ← paste the full identity cookie value here

    private const int AuthProtocolVersion = 3;
    private const int PingIntervalSeconds = 5;

    // Asset to subscribe to
    private const int ActiveId =(int)EnumMarketAssetId.Gold; // Gold (1861 = EUR/USD, observed in live traffic)
    private const EnumIQOptionCandleSize CandleSize = EnumIQOptionCandleSize.FiveSeconds;
    private const int HistoricalCandleCount = 30; // how many history candles to keep printing
    private const int AuthWaitSeconds = 20;       // server sometimes replies a few seconds late

    private static long _localTimeOrigin = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    private int _requestSeq;
    private TaskCompletionSource<bool>? _authTcs;
    private string? _firstCandlesRequestId;
    private int _liveCandleCount;

    internal async Task Run()
    {
        ValidateSSID();
        using var ws = new ClientWebSocket();
        SetIdentityCookie(ws);
        var isConnected = await Connecting(ws);
        if (!isConnected) return;

        using var cts = new CancellationTokenSource();
        var pingLoop = Task.Run(() => StartPingLoopAsync(ws, cts.Token));

        try
        {
            await Authenticate(ws);
            await HistoricalCandle(ws);
            await SubscribeToTheLiveCandleStream(ws);
            await ReceiveLoopAsync(ws);
        }
        finally
        {
            await CloseConnections(ws, cts);
        }
    }

    private static async Task CloseConnections(ClientWebSocket ws, CancellationTokenSource cts)
    {
        cts.Cancel();
        if (ws.State == WebSocketState.Open)
        {
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "done", CancellationToken.None);
        }
    }

    private async Task SubscribeToTheLiveCandleStream(ClientWebSocket ws)
    {
        // ── 2. Subscribe to the live candle stream (quotes.candle-generated) ──────
        var liveSubscribe = new SubscribeMessageEnvelope
        {
            Msg = new SubscribeMessageBody
            {
                Name = "quotes.candle-generated",
                Params = new SubscribeParams
                {
                    RoutingFilters = new CandleRoutingFilters
                    {
                        ActiveId = ActiveId,
                        Size = (int)CandleSize
                    }
                },
                Version = "1.0"
            },
            RequestId = NextRequestId(),
            LocalTime = LocalTimeMs()
        };
        await SendRawAsync(ws, JsonConvert.SerializeObject(liveSubscribe));
        Console.WriteLine("Subscribed to live candle stream: active_id={0}, size={1}s",
            ActiveId, (int)CandleSize);
    }

    private async Task HistoricalCandle(ClientWebSocket ws)
    {
        // ── 1. Historical candles (get-first-candles: needs ONLY active_id) ───────
        var firstCandles = new QuotesHistoryGetFirstCandlesRequest(ActiveId);
        var firstCandlesId = NextRequestId();
        _firstCandlesRequestId = firstCandlesId;
        await SendRawAsync(ws, JsonConvert.SerializeObject(
            SocketSendMessageEnvelope.Wrap(firstCandles, firstCandlesId, LocalTimeMs())));
        Console.WriteLine("Sent quotes-history.get-first-candles for active_id={0}", ActiveId);
    }

    private async Task Authenticate(ClientWebSocket ws)
    {
        // ── 0. Authenticate (required before the server will stream real data) ──
        var authTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _authTcs = authTcs;
        await SendAuthAsync(ws);


        var authenticated = (await Task.WhenAny(authTcs.Task, Task.Delay(AuthWaitSeconds * 1000))) == authTcs.Task
            && await authTcs.Task;
        Console.WriteLine(authenticated
            ? "✔ Authenticated successfully."
            : $"[!] No 'authenticated' reply within {AuthWaitSeconds}s — candle stream may be empty.");
    }

    private static async Task<bool> Connecting(ClientWebSocket ws)
    {
        Console.WriteLine("Connecting to {0} ...", WebSocketUrl);
        var connectTask = ws.ConnectAsync(new Uri(WebSocketUrl), CancellationToken.None);
        if (!connectTask.Wait(TimeSpan.FromSeconds(15)))
        {
            Console.WriteLine("Connection timed out.");
            return false;
        }
        await connectTask;
        Console.WriteLine("Connected. State: {0}", ws.State);
        return true;
    }

    private static void SetIdentityCookie(ClientWebSocket ws)
    {
        // .NET 8 ClientWebSocket cannot set arbitrary request headers (Origin/User-Agent),
        // BUT it CAN set the Cookie header, which is what IQ Option actually checks.
        if (!string.IsNullOrEmpty(IdentityCookie))
        {
            ws.Options.SetRequestHeader("Cookie", $"identity={IdentityCookie}");
            ws.Options.SetRequestHeader("User-Agent",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0 Safari/537.36");
        }
    }

    private static void ValidateSSID()
    {
        if (string.IsNullOrEmpty(Ssid))
        {
            Console.WriteLine("[!] Ssid is empty — authentication will fail.");
            Console.WriteLine("    Paste the 'ssid' cookie value from km.iqoption.com into the const.");
        }
    }

    // ── Helpers ────────────────────────────────────────────────────────────────
    private static async Task SendAuthAsync(ClientWebSocket ws)
    {
        var auth = new SocketAuthenticateEnvelope
        {
            Msg = new SocketAuthenticateBody { Ssid = Ssid, Protocol = AuthProtocolVersion },
            RequestId = "request_2",
            LocalTime = LocalTimeMs()
        };
        await SendRawAsync(ws, JsonConvert.SerializeObject(auth));
        Console.WriteLine("Sent authenticate frame (ssid={0}).", Ssid);
    }
    private static async Task StartPingLoopAsync(ClientWebSocket ws, CancellationToken token)
    {
        while (!token.IsCancellationRequested && ws.State == WebSocketState.Open)
        {
            try
            {
                await SendRawAsync(ws, JsonConvert.SerializeObject(SocketPingFrame.Create()));
            }
            catch
            {
                break;
            }
            await Task.Delay(TimeSpan.FromSeconds(PingIntervalSeconds), token);
        }
    }

    private async Task ReceiveLoopAsync(ClientWebSocket ws)
    {
        // ── 3. Listen loop: print frames, parse candle responses ─────────
        var buffer = new byte[64 * 1024];
        var ms = new MemoryStream();

        while (ws.State == WebSocketState.Open)
        {
            ms.SetLength(0);
            WebSocketReceiveResult result;
            do
            {
                result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("Server closed the connection: {0}", result.CloseStatus);
                    return;
                }
                ms.Write(buffer, 0, result.Count);
            } while (!result.EndOfMessage);

            var text = Encoding.UTF8.GetString(ms.GetBuffer(), 0, (int)ms.Length);
            await HandleIncomingFrame(text);
        }
    }

    private async Task HandleIncomingFrame(string raw)
    {
        // Telemetry/echo traffic we don't care about right now
        if (raw.Contains("\"resource\":\"events") || raw.Contains("\"resource\":\"ping"))
        {
            return;
        }

        // Fast-path: signal the authenticate handshake completion
        if (raw.Contains("\"name\":\"authenticated\""))
        {
            Console.WriteLine("[auth] server replied: {0}", Truncate(raw, 200));
            _authTcs?.TrySetResult(raw.Contains("\"msg\":true"));
            return;
        }

        // High-frequency heartbeat — suppress to keep the log readable
        if (raw.Contains("\"name\":\"timeSync\"")) return;

        Console.WriteLine("[recv] {0}", Truncate(raw, 500));

        // Parse the generic envelope {name, msg, request_id, ...}
        try
        {
            var envelope = JsonConvert.DeserializeObject<SocketEnvelopeGeneric>(raw);
            if (envelope?.Msg is null) return;

            // (A) LIVE tick pushed per candle: top-level name == "candle-generated"
            if (envelope.Name == "candle-generated")
            {
                var c = JsonConvert.DeserializeObject<IQOptionCandle>(
                    JsonConvert.SerializeObject(envelope.Msg));
                if (c is not null) PrintCandle(c, "live");
                return;
            }

            // (B) HISTORICAL batch reply: top-level name == "candles", msg = {"candles":[...]}
            //     confirmed reply: {"request_id":"request_49","name":"candles","msg":{"candles":[]},"status":2000}
            if (envelope.Name == "candles")
            {
                var response = JsonConvert.DeserializeObject<GetCandlesResponse>(
                    JsonConvert.SerializeObject(envelope.Msg));
                if (response?.Candles is { Count: > 0 } candles)
                {
                    Console.WriteLine("── history (get-first-candles): {0} candle(s) ──", candles.Count);
                    var toPrint = candles.Take(HistoricalCandleCount);
                    foreach (var cc in toPrint) PrintCandle(cc, "hist");
                    if (candles.Count > HistoricalCandleCount)
                        Console.WriteLine("    … {0} more", candles.Count - HistoricalCandleCount);
                }
                else
                {
                    Console.WriteLine("── history reply: {0} candle(s) (empty?) ──", response?.Candles?.Count ?? 0);
                }
                return;
            }
        }
        catch (JsonException)
        {
            // Not JSON we recognize — already printed raw above
        }
    }

    private static void PrintCandle(IQOptionCandle c, string kind)
    {
        var from = c.At > 0 ? DateTimeOffset.FromUnixTimeMilliseconds(c.At / 1_000_000).UtcDateTime : DateTime.MinValue;
        Console.WriteLine("  [{0}] id={1} {2:HH:mm:ss.fff}  O={3} H={4} L={5} C={6}  bid={7} ask={8} V={9} phase={10}",
            kind, c.Id, from, c.Open, c.Max, c.Min, c.Close, c.Bid, c.Ask, c.Volume, c.Phase);
    }

    private string NextRequestId() => $"request_{Interlocked.Increment(ref _requestSeq)}";

    private static long LocalTimeMs() => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - _localTimeOrigin;

    private static string Truncate(string s, int max) => s.Length <= max ? s : s[..max] + "…";

    private static async Task SendRawAsync(ClientWebSocket ws, string json)
    {
        var bytes = Encoding.UTF8.GetBytes(json);
        await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
    }
}

public enum EnumMarketAssetId
{
    Gold = 1912,
    EURUSD = 1861
}


// ═══════════════════════════════════════════════════════════════════════════════
// Enums
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Candle interval in seconds as used by the IQ Option PWA quotes protocol
/// (the "size" field of quotes-history.get-candles).
/// </summary>
public enum EnumIQOptionCandleSize
{
    FiveSeconds = 5,
    TenSeconds = 10,
    FifteenSeconds = 15,
    ThirtySeconds = 30,
    OneMinute = 60,
    TwoMinutes = 120,
    FiveMinutes = 300,
    TenMinutes = 600,
    FifteenMinutes = 900,
    ThirtyMinutes = 1800,
    OneHour = 3600,
    FourHours = 14400,
    OneDay = 86400
}

// ═══════════════════════════════════════════════════════════════════════════════
// Model: candles request
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Inner message for the ONE-SHOT historical fetch. Verified 2026-08-19 — the web client
/// uses "quotes-history.get-first-candles" (NOT "get-candles") to load a chart.
/// Wire format: {"name":"quotes-history.get-first-candles","version":"1.0","body":{"active_id":1912}}
/// Response:    {"request_id":"request_49","name":"candles","msg":{"candles":[...]},"status":2000}
/// Only "active_id" is required — no from_id/tail id needed (this is why v2.0 get-candles
/// with from_id=0 returned an empty list).
/// </summary>
public class QuotesHistoryGetFirstCandlesRequest
{
    [JsonProperty("name")]
    public string Name { get; set; } = "quotes-history.get-first-candles";

    [JsonProperty("version")]
    public string Version { get; set; } = "1.0";

    [JsonProperty("body")]
    public QuotesHistoryGetFirstCandlesBody Body { get; set; } = new();

    public QuotesHistoryGetFirstCandlesRequest(int activeId) => Body.ActiveId = activeId;
}

public class QuotesHistoryGetFirstCandlesBody
{
    [JsonProperty("active_id")]
    public int ActiveId { get; set; }
}

/// <summary>
/// Inner message to open the LIVE candle stream. Verified 2026-08-19.
/// Wire format: {"name":"quotes.candle-generated","params":{"routingFilters":{"active_id":1912,"size":5}},"version":"1.0"}
/// The server then pushes {"name":"candle-generated","msg":{...}} frames every tick.
/// </summary>
public class SubscribeMessageBody
{
    [JsonProperty("name")]
    public string Name { get; set; } = "quotes.candle-generated";

    [JsonProperty("params")]
    public SubscribeParams Params { get; set; } = new();

    [JsonProperty("version")]
    public string Version { get; set; } = "1.0";
}

public class SubscribeParams
{
    [JsonProperty("routingFilters")]
    public CandleRoutingFilters RoutingFilters { get; set; } = new();
}

public class CandleRoutingFilters
{
    [JsonProperty("active_id")]
    public int ActiveId { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Model: socket envelope & ping
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Outer envelope every application-level message is wrapped in before hitting the socket.
/// Wire format (observed 2026-08-19):
/// {"name":"sendMessage","msg":{...actual message...},"request_id":"request_433","local_time":2722686}
/// </summary>
public class SocketSendMessageEnvelope
{
    [JsonProperty("name")]
    public string Name { get; set; } = "sendMessage";

    [JsonProperty("msg")]
    public object Msg { get; set; } = null!;

    /// <summary>Client-generated correlation id, e.g. "request_433".</summary>
    [JsonProperty("request_id")]
    public string RequestId { get; set; } = string.Empty;

    /// <summary>Client time in ms (app-relative timestamp is what the web client sends).</summary>
    [JsonProperty("local_time")]
    public long LocalTime { get; set; }

    public static SocketSendMessageEnvelope Wrap(object msg, string requestId, long localTime)
        => new() { Msg = msg, RequestId = requestId, LocalTime = localTime };
}

/// <summary>
/// Outer envelope for subscription requests (distinct from "sendMessage").
/// Wire format (observed 2026-08-19):
/// {"name":"subscribeMessage","msg":{"name":"quotes.candle-generated","params":{"routingFilters":{"active_id":1912,"size":5}},"version":"1.0"},"request_id":"request_50","local_time":5531}
/// </summary>
public class SubscribeMessageEnvelope
{
    [JsonProperty("name")]
    public string Name { get; set; } = "subscribeMessage";

    [JsonProperty("msg")]
    public SubscribeMessageBody Msg { get; set; } = new();

    [JsonProperty("request_id")]
    public string RequestId { get; set; } = string.Empty;

    [JsonProperty("local_time")]
    public long LocalTime { get; set; }
}

/// <summary>
/// Generic incoming frame. Every application-level message the server pushes has this
/// shape: {"name":"<event>","msg":{...},"request_id":"...","..."}
/// Used to decode the live "candle-generated" stream and get-candles results.
/// </summary>
public class SocketEnvelopeGeneric
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("msg")]
    public object? Msg { get; set; }

    [JsonProperty("request_id")]
    public string? RequestId { get; set; }

    [JsonProperty("microserviceName")]
    public string? MicroserviceName { get; set; }
}

/// <summary>
/// Post-connect authentication frame. Sent once right after the socket opens.
/// Wire format (observed 2026-08-19):
/// {"name":"authenticate","msg":{"ssid":"24c1...f9ch","protocol":3},"request_id":"request_2","local_time":2228}
/// The server answers {"name":"authenticated","msg":true,...}.
/// </summary>
public class SocketAuthenticateEnvelope
{
    [JsonProperty("name")]
    public string Name { get; set; } = "authenticate";

    [JsonProperty("msg")]
    public SocketAuthenticateBody Msg { get; set; } = new();

    [JsonProperty("request_id")]
    public string RequestId { get; set; } = "request_2";

    [JsonProperty("local_time")]
    public long LocalTime { get; set; }
}

public class SocketAuthenticateBody
{
    [JsonProperty("ssid")]
    public string Ssid { get; set; } = string.Empty;

    [JsonProperty("protocol")]
    public int Protocol { get; set; } = 3;
}

/// <summary>
/// Keep-alive frame the server expects roughly every 5 seconds, otherwise the connection drops.
/// Wire format: {"resource":"ping","timestamp":1787152222559}
/// </summary>
public class SocketPingFrame
{
    [JsonProperty("resource")]
    public string Resource { get; set; } = "ping";

    [JsonProperty("timestamp")]
    public long Timestamp { get; set; } = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

    public static SocketPingFrame Create() => new();
}

// ═══════════════════════════════════════════════════════════════════════════════
// Model: candles response
// ═══════════════════════════════════════════════════════════════════════════════

/// <summary>
/// Response envelope for "quotes-history.get-candles". The historical batch, when
/// returned, arrives wrapped in the standard {name, msg:{...}} envelope; msg is
/// expected to carry {active_id, size, candles:[...]} (confirm exact nesting once a
/// real get-candles reply is captured — the live stream is confirmed, the batch is not).
/// </summary>
public class GetCandlesResponse
{
    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("request_id")]
    public string? RequestId { get; set; }

    [JsonProperty("active_id")]
    public int? ActiveId { get; set; }

    [JsonProperty("size")]
    public int? Size { get; set; }

    [JsonProperty("candles")]
    public List<IQOptionCandle>? Candles { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }

    [JsonProperty("body")]
    public GetCandlesResponseBody? Body { get; set; }

    /// <summary>Flattened accessor: candles from either .candles or .body.candles.</summary>
    [JsonIgnore]
    public List<IQOptionCandle>? EffectiveCandles => Candles ?? Body?.Candles;

    [JsonIgnore]
    public string? EffectiveError => Error ?? Body?.Error;
}

public class GetCandlesResponseBody
{
    [JsonProperty("active_id")]
    public int? ActiveId { get; set; }

    [JsonProperty("size")]
    public int? Size { get; set; }

    [JsonProperty("candles")]
    public List<IQOptionCandle>? Candles { get; set; }

    [JsonProperty("error")]
    public string? Error { get; set; }
}

/// <summary>
/// A single candle. Confirmed live shape from a "candle-generated" frame (2026-08-19):
/// {"active_id":1912,"size":5,"at":1787154461000000000,"from":1787154460,"to":1787154465,
///  "id":10976910,"open":4493.205,"close":4493.035,"min":4492.995,"max":4493.325,
///  "ask":4493.16,"bid":4492.91,"volume":26,"phase":"T"}
/// NOTE: "at" is in NANOseconds; "min"/"max" are the low/high; "bid"/"ask" are the live quotes.
/// </summary>
public class IQOptionCandle
{
    [JsonProperty("active_id")]
    public int ActiveId { get; set; }

    [JsonProperty("size")]
    public int Size { get; set; }

    /// <summary>Candle open time in NANOseconds since epoch.</summary>
    [JsonProperty("at")]
    public long At { get; set; }

    [JsonProperty("from")]
    public long From { get; set; }

    [JsonProperty("to")]
    public long To { get; set; }

    [JsonProperty("id")]
    public long Id { get; set; }

    [JsonProperty("open")]
    public decimal Open { get; set; }

    [JsonProperty("close")]
    public decimal Close { get; set; }

    [JsonProperty("min")]
    public decimal Min { get; set; }

    [JsonProperty("max")]
    public decimal Max { get; set; }

    [JsonProperty("bid")]
    public decimal Bid { get; set; }

    [JsonProperty("ask")]
    public decimal Ask { get; set; }

    [JsonProperty("volume")]
    public long Volume { get; set; }

    [JsonProperty("phase")]
    public string? Phase { get; set; }
}

