using System;
using System.Net;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace TestConsole.Programs;

public class ServerLogExporer
{
    public static async Task Run()
    {
        var isExisting = false;
        var logServerUrl = "http://ccwl-log.remotes.local/";
        var selected = new DirEntry { Name = "Root", Href = "/", IsDirectory = true };

        AnsiConsole.MarkupLine("[bold cyan]=== Log Viewer ===[/]");
        AnsiConsole.MarkupLine("Browse and search server logs from [bold]ccwl-log.remotes.local[/]\n");

        do
        {
            // Step 1: Show list of projects
            var projectEntries = await FetchEntriesAsync($"{logServerUrl.TrimEnd('/')}{selected.Href}", "/");
            if (projectEntries == null || projectEntries.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No projects found.[/]");
                return;
            }

            projectEntries.Add(new DirEntry { Name = "[Back]", Href = "", IsDirectory = false });
            projectEntries.Add(new DirEntry { Name = "[Exit]", Href = "", IsDirectory = false });

            selected = AnsiConsole.Prompt(
                new SelectionPrompt<DirEntry>()
                    .Title("Select a project")
                    .PageSize(15)
                    .MoreChoicesText("[grey](Move up and down to view more projects)[/]")
                    .UseConverter(p => Markup.Escape(p.Name))
                    .AddChoices(projectEntries)
                    .WrapAround()
            );

            if (selected.Name == "[Exit]")
            {
                AnsiConsole.MarkupLine("[green]Exiting...[/]");
                return;
            }
            else if (selected.Name == "[Back]")
            {
                selected.Href = selected.Href.Substring(0, selected.Href.LastIndexOf('/') + 1);
                continue;
            }

            AnsiConsole.MarkupLine($"[green]Selected: {Markup.Escape(selected.Name)}[/]\n");
        } while (!isExisting);
    }

    private static async Task<List<DirEntry>> FetchEntriesAsync(string logServerUrl, string v)
    {
        var entries = await FetchDirectoryEntriesAsync(logServerUrl, v);
        if (entries == null || entries.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No entries found.[/]");
            return new List<DirEntry>();
        }

        return entries;
    }

    #region HTTP Helpers

    private static async Task DisplayLogContentAsync(string baseUrl, DirEntry file, string searchQuery)
    {
        var url = $"{baseUrl.TrimEnd('/')}{file.Href}";
        var content = await FetchFileContentAsync(url);

        if (string.IsNullOrWhiteSpace(content))
        {
            AnsiConsole.MarkupLine("[yellow]Empty log file.[/]");
            return;
        }

        var lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var filteredLines = lines.Where(l => l.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();
            lines = filteredLines.ToArray();

            AnsiConsole.MarkupLine($"[bold]Found {lines.Length} matching line(s):[/]\n");
        }
        else
        {
            AnsiConsole.MarkupLine($"[bold]Showing all {lines.Length} line(s):[/]\n");
        }

        // Display in a table or text panel
        var maxLines = 200; // Limit display to avoid overwhelming output
        var displayLines = lines.Take(maxLines).ToArray();

        if (lines.Length > maxLines)
        {
            AnsiConsole.MarkupLine($"[grey]Showing first {maxLines} of {lines.Length} lines...\n[/]");
        }

        // Create a text panel for the log content
        var logText = string.Join("\n", displayLines);
        AnsiConsole.MarkupLine($"[dim]{Markup.Escape(logText)}[/]");

        if (lines.Length > maxLines)
        {
            AnsiConsole.MarkupLine("\n[yellow](End of display. Use a smaller query or view the full file directly.)[/]");
        }
    }

    #endregion

    #region Web Scraping Helpers

    // The log server exposes classic IIS directory browsing: a single <pre> block
    // per page, entries separated by <br>, e.g.:
    //   3/19/2019 10:42 PM        &lt;dir&gt; <A HREF="/App_Data/">App_Data</A><br>
    //   3/5/2026 12:16 PM          566 <A HREF="/web.config">web.config</A><br>
    private static readonly Regex EntryPattern = new(
        @"\d{1,2}/\d{1,2}/\d{4}\s+\d{1,2}:\d{2}\s*(?:AM|PM)\s+(?<size>&lt;dir&gt;|\d+)\s+<A\s+HREF=""(?<href>[^""]*)""[^>]*>(?<name>.*?)</A>",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static async Task<List<DirEntry>?> FetchDirectoryEntriesAsync(string baseUrl, string href)
    {
        var url = $"{baseUrl.TrimEnd('/')}{href}";
        var html = await FetchWebPageAsync(url);
        if (string.IsNullOrEmpty(html)) return null;

        var entries = new List<DirEntry>();
        foreach (Match match in EntryPattern.Matches(html))
        {
            var sizeToken = match.Groups["size"].Value;
            var isDirectory = sizeToken.Equals("&lt;dir&gt;", StringComparison.OrdinalIgnoreCase);
            entries.Add(new DirEntry
            {
                Name = WebUtility.HtmlDecode(match.Groups["name"].Value.Trim()),
                Href = match.Groups["href"].Value.Trim(),
                IsDirectory = isDirectory,
                Size = isDirectory ? 0 : long.Parse(sizeToken)
            });
        }

        return entries.Count > 0 ? entries : null;
    }

    private static async Task<string> FetchFileContentAsync(string url) => await FetchWebPageAsync(url);

    private static async Task<string> FetchWebPageAsync(string url)
    {
        try
        {
            using var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Error fetching {Markup.Escape(url)}: {Markup.Escape(ex.Message)}[/]");
            return string.Empty;
        }
    }

    #endregion

    #region Utilities

    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }

    #endregion
}

public class DirEntry
{
    public string Name { get; set; } = "";
    public string Href { get; set; } = "";
    public bool IsDirectory { get; set; }
    public long Size { get; set; }
}