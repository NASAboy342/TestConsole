
using System.Text;
using Newtonsoft.Json;
using TestConsole.Programs;

public class Program
{
    public static async Task Main()
    {
        var utopiaHacksawGaming = new UtopiaHacksawGaming();
        var gameId = 68;
        await utopiaHacksawGaming.TestAllCurrencyByGameDemo(gameId);
    }
}
