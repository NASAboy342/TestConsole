
using System.Text;
using Newtonsoft.Json;
using TestConsole.Programs;

public class Program
{
    public static async Task Main()
    {
        var balance = 123.45600000000000m;
        var formattedBalance = GetProviderDecimalFormat(balance);
        Console.WriteLine(formattedBalance);
    }
    private static decimal GetProviderDecimalFormat(decimal balance)
    {
        // 6 decimal places
        return Math.Round(balance, 6);
    }
}
