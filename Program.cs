
using System.Net;
using System.Text.RegularExpressions;
using Spectre.Console;
using TestConsole.Programs;

public class Program
{
    public static async Task Main()
    {
        var aitherBlockIpTester = new AitherBlockIpTester();
        await aitherBlockIpTester.Run();
    }
}