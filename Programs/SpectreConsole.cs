using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spectre.Console;

namespace TestConsole.Programs
{
    public class SpectreConsole
    {
        public void Run()
        {
            // Create a table
            var table = new Table();

            // Add some columns
            table.AddColumn("Date");
            table.AddColumn("Summary");
            table.AddColumn("Temperature (C)");
            table.AddColumn("Temperature (F)");

            // Add some rows
            table.AddRow("2024-04-16", "Sunny", "15", "59");
            table.AddRow("2024-04-17", "Cloudy", "17", "62.6");
            table.AddRow("2024-04-18", "Rainy", "13", "55.4");

            // Render the table to the console
            AnsiConsole.Render(table);
        }
    }
}
