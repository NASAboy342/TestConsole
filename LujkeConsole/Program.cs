// ssid: 24c1a7c8e53fe13e1ea825be5248f9ch
using Spectre.Console;

var ssid = AnsiConsole.Ask<string>("Enter your IQ Option SSID: ");
var identityCookie = AnsiConsole.Ask<string>("Enter your IQ Option Identity Cookie: ");
var selectedMarket = AnsiConsole.Prompt( new MultiSelectionPrompt<string>().Title("Select the markets you want to subscribe to:").AddChoices("Gold", "EURUSD"));