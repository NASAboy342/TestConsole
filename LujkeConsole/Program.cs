using LujkeConsole.Services;
using Spectre.Console;
using Spectre.Console.Rendering;

const int ChartWidth = 100;
const int ChartHeight = 30;
const int MaxLogLines = 6;

var ssid = AnsiConsole.Ask<string>("Enter your IQ Option SSID: ");
var identityCookie = AnsiConsole.Ask<string>("Enter your IQ Option Identity Cookie: ");
var selectedMarket = AnsiConsole.Prompt( new SelectionPrompt<string>().Title("Select the markets you want to subscribe to:").AddChoices(Enum.GetNames(typeof(EnumMarketAssetId))));
var marketAssetId = EnumMarketAssetId.Gold;
if(!Enum.TryParse<EnumMarketAssetId>(selectedMarket, out marketAssetId))
{
    AnsiConsole.MarkupLine($"[red]Invalid market selection: {selectedMarket}[/]");
    return;
}

AnsiConsole.Clear();

var iqOptionService = new IqOptionService(ssid, identityCookie, (int)marketAssetId);
var logMessages = new Queue<string>();

IRenderable BuildDashboard()
{
    var candles = iqOptionService.LiveCandles.TakeLast(ChartWidth).ToList();
    var chartRows = BuildCandleChart(candles);
    var chart = new Panel(new Markup(string.Join("\n", chartRows)))
        .Header($" {selectedMarket} - last {candles.Count} candles ")
        .Border(BoxBorder.Rounded);
    var logs = new Panel(new Markup(string.Join("\n", logMessages.Select(Markup.Escape))))
        .Header(" Activity ")
        .Border(BoxBorder.Rounded);

    return new Rows(chart, logs);
}

IEnumerable<string> BuildCandleChart(IReadOnlyList<IQOptionCandle> candles)
{
    if (candles.Count == 0)
        return new[] { "Waiting for candle data..." };

    var low = candles.Min(candle => candle.Min);
    var high = candles.Max(candle => candle.Max);
    var range = high - low;
    if (range == 0)
        range = 1;

    var rows = new List<string>();
    for (var row = ChartHeight - 1; row >= 0; row--)
    {
        var price = low + range * row / (ChartHeight - 1);
        var cells = candles.Select(candle => FormatCandleCell(candle, price, range / (ChartHeight - 1)));
        rows.Add($"[grey]{price,10:F5}[/] {string.Concat(cells)}");
    }

    var times = string.Concat(candles.Select(candle =>
        DateTimeOffset.FromUnixTimeSeconds(candle.From).ToLocalTime().ToString("mm")[^1]));
    rows.Add($"[grey]{string.Empty,10}[/] {times}");
    return rows;
}

string FormatCandleCell(IQOptionCandle candle, decimal price, decimal step)
{
    var isWick = price >= candle.Min - step / 2 && price <= candle.Max + step / 2;
    var bodyLow = decimal.Min(candle.Open, candle.Close);
    var bodyHigh = decimal.Max(candle.Open, candle.Close);
    var isBody = price >= bodyLow - step / 2 && price <= bodyHigh + step / 2;
    var color = candle.Close >= candle.Open ? "green" : "red";

    return isBody ? $"[{color}]█[/]" : isWick ? $"[{color}]│[/]" : " ";
}

var dashboard = new Layout("dashboard");
dashboard.Update(BuildDashboard());

await AnsiConsole.Live(dashboard)
    .AutoClear(false)
    .StartAsync(async context =>
    {
        void AddLog(string message)
        {
            logMessages.Enqueue(message);
            while (logMessages.Count > MaxLogLines)
                logMessages.Dequeue();
            dashboard.Update(BuildDashboard());
            context.Refresh();
        }

        iqOptionService.OnLogMessage += (_, message) => AddLog(message);
        iqOptionService.OnError += (_, message) => AddLog($"ERROR: {message}");
        iqOptionService.OnCandleUpdated += (_, _) =>
        {
            dashboard.Update(BuildDashboard());
            context.Refresh();
        };

        await iqOptionService.Run();
    });



