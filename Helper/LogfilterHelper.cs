using System;

namespace TestConsole.Helper;

public class LogfilterHelper
{
    internal async Task Run(string filePath, string logLevel, string filter)
    {
        var logEntries = new List<string>();

        using (var reader = new StreamReader(filePath))
        {
            string line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line.Contains(logLevel) && line.Contains(filter))
                {
                    logEntries.Add(line);
                }
            }
        }

        Console.WriteLine($"Found {logEntries.Count} log entries for user {filter} with log level {logLevel}:");
        foreach (var entry in logEntries)
        {
            Console.WriteLine(entry);
        }
    }
}
