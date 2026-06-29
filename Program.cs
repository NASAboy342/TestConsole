
using System.Globalization;
using Newtonsoft.Json;
using TestConsole.Helper;
using TestConsole.Programs;

public class Program
{
    public static async Task Main()
    {
        var ollama = new OllamaClient();
        await ollama.Run();
    }
}
