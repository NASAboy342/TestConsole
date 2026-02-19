
using System.Text;
using DocumentFormat.OpenXml.Drawing;
using Newtonsoft.Json;
using TestConsole.Programs;

public class Program
{
    public static async Task Main()
    {
        // var utopiaTestLogin = new UtopiaTestLogin(1102, "https://capi-uat-qtech.csmc-api.com");
        // await utopiaTestLogin.TestAllGameAndAllOfItsCurrencyDemo();

        var gameListHelper = new GamelistHelper();
        await gameListHelper.Run();
    }
}