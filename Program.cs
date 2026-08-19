
using TestConsole.Programs;

namespace TestConsole;

public class Program
{
    public static async Task Main()
    {
        var iqOptionScraper = new IQOptionScraper();
        await iqOptionScraper.Run();
    }
}