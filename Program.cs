
using System.Text;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using TestConsole.Programs;

public class Program
{
    public static async Task Main()
    {
        var utopiatest = new UtopiaTestLogin();
        await utopiatest.TestAllGameByCurrencyDemo("INR");
    }
}