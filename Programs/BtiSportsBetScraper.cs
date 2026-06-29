using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace TestConsole.Programs;

public class BtiSportsBetScraper
{
    private const string BtiUrl = "https://bti360.bti-sports.io/";
    private const string BetHistoryUrl = "https://bti360.bti-sports.io/reports/bet-history";
    private const string Email = "games-support@568win.com";
    private const string Password = "!QAZ2wsx3edc";
    private const string Corporate = "568win";
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    private readonly int _batchSize = 15;
    private readonly bool _isHeadless = false;
    private readonly int _perPage = 500;

    // ── Single bet lookup by Purchase ID ─────────────────────────────────────

    public async Task<BtiPurchaseDetail?> GetBetDetailAsync(string purchaseId)
    {
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await LoginAsync(page);
        return await SearchAndScrapeAsync(page, purchaseId);
    }

    private async Task LoginAsync(IPage page)
    {
        // Navigate — BTI does a JS redirect to Auth0, so wait for the email input
        // rather than the URL (the JS redirect fires after GotoAsync resolves)
        await page.GotoAsync(BtiUrl);
        await page.GetByPlaceholder("yours@example.com").WaitForAsync(new() { Timeout = 30_000 });

        // Fill credentials and submit
        await page.GetByPlaceholder("yours@example.com").FillAsync(Email);
        await page.GetByPlaceholder("your password").FillAsync(Password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();

        // Wait for corporate picker dialog to appear
        await page.WaitForSelectorAsync("text=Choose corporate", new() { Timeout = 15_000 });

        // Open the custom dropdown (a plain div with "Corporate: Please Choose" text)
        await page.GetByText("Corporate: Please Choose").ClickAsync();
        await page.GetByRole(AriaRole.Menuitem, new() { Name = Corporate }).ClickAsync();

        // Final login
        await page.GetByRole(AriaRole.Button, new() { Name = "Log In" }).ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 15_000 });
    }

    private async Task<BtiPurchaseDetail?> SearchAndScrapeAsync(IPage page, string purchaseId)
    {
        // Type into the search box and submit (scope Search button to the header banner to avoid ambiguity)
        await page.GetByRole(AriaRole.Textbox, new() { Name = "Type in Purchase ID/ Bet ID." }).FillAsync(purchaseId);
        await page.GetByRole(AriaRole.Banner).GetByRole(AriaRole.Button, new() { Name = "Search", Exact = true }).ClickAsync();

        // Wait explicitly for a <td> containing our Purchase ID to appear (table is AJAX-loaded)
        var resultCell = page.Locator($"td:has-text('{purchaseId}')").First;
        try
        {
            await resultCell.WaitForAsync(new() { Timeout = 20_000 });
        }
        catch (TimeoutException)
        {
            Log($"[BTI] No result found for Purchase ID: {purchaseId}");
            return null;
        }

        // Find the row that contains our purchase ID
        var row = page.Locator("table tbody tr").Filter(new LocatorFilterOptions { HasText = purchaseId }).First;
        var cells = row.Locator("td");
        var detail = await MapCellToBtiPurchaseDetial(cells);

        return detail;
    }

    private static async Task<BtiPurchaseDetail> MapCellToBtiPurchaseDetial(ILocator cells)
    {
        return new BtiPurchaseDetail
        {
            PurchaseId = await GetCellTextAsync(cells, 0),
            BetId = await GetCellTextAsync(cells, 1),
            BetDate = await GetCellTextAsync(cells, 2),
            Operator = await GetCellTextAsync(cells, 3),
            CustomerId = await GetCellTextAsync(cells, 4),
            MerchantCustomerCode = await GetCellTextAsync(cells, 5),
            UserName = await GetCellTextAsync(cells, 6),
            BetType = await GetCellTextAsync(cells, 8),
            Sport = await GetCellTextAsync(cells, 9),
            League = await GetCellTextAsync(cells, 10),
            Event = await GetCellTextAsync(cells, 11),
            Selection = await GetCellTextAsync(cells, 12),
            Stake = await GetCellTextAsync(cells, 13),
            WinLoss = await GetCellTextAsync(cells, 14),
            BetStatus = await GetCellTextAsync(cells, 16),
            SettledDate = await GetCellTextAsync(cells, 17),
            IsFreeBet = (await GetCellTextAsync(cells, 18)).Equals("Yes", StringComparison.OrdinalIgnoreCase)
        };
    }

    // ── Batch lookup by Purchase IDs ─────────────────────────────────────────

    /// <summary>
    /// Returns bet details for each Purchase ID in <paramref name="purchaseIds"/>.
    /// A single browser session is reused across all lookups.
    /// IDs that return no result are silently skipped.
    /// </summary>
    public async Task<List<BtiPurchaseDetail>> GetBetDetailsByPurchaseIdsAsync(IEnumerable<string> purchaseIds)
    {
        var ids = purchaseIds.ToList();
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await LoginAsync(page);

        var results = new List<BtiPurchaseDetail>(ids.Count);

        for (int i = 0; i < ids.Count; i++)
        {
            var id = ids[i];
            Log($"[BTI] [{i + 1}/{ids.Count}] Looking up Purchase ID: {id}");
            var detail = await SearchAndScrapeAsync(page, id);
            if (detail is not null)
                results.Add(detail);
        }

        Log($"[BTI] Done — {results.Count}/{ids.Count} bet details retrieved.");
        return results;
    }

    // ── Date-range batch lookup ───────────────────────────────────────────────

    /// <summary>
    /// Returns all bets placed between <paramref name="startDate"/> and <paramref name="endDate"/> (inclusive).
    /// Dates are treated as UTC. Format used by BTI BO: "yyyy-MM-dd HH:mm".
    /// </summary>
    public async Task<List<BtiPurchaseDetail>> GetBetsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        startDate = startDate.ToUniversalTime();
        endDate = endDate.ToUniversalTime();
        using var playwright = await Playwright.CreateAsync();
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = _isHeadless
        });

        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        await LoginAsync(page);
        return await SearchByDateRangeAndScrapeAsync(page, startDate, endDate);
    }

    private async Task<List<BtiPurchaseDetail>> SearchByDateRangeAndScrapeAsync(
        IPage page, DateTime startDate, DateTime endDate)
    {
        await page.GotoAsync(BetHistoryUrl);
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // ── Ensure the filter panel is expanded so date inputs are accessible ──
        // "Expand Filter" text is visible when the panel is COLLAPSED.
        // We click it to open the panel, then wait for that text to disappear
        // (which confirms the panel is now fully open).
        var expandFilterLoc = page.Locator("text=Expand Filter");
        if (await expandFilterLoc.IsVisibleAsync())
        {
            await expandFilterLoc.ClickAsync();
            await expandFilterLoc.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 5_000 });
        }

        // ── Set date range (MUI overlay intercepts normal clicks → Force = true) ──
        var startInput = page.Locator("input[placeholder='YYYY-MM-DD HH:mm']").First;
        var endInput   = page.Locator("input[placeholder='YYYY-MM-DD HH:mm']").Nth(1);

        var startVal = startDate.ToString("yyyy-MM-dd HH:mm");
        var endVal   = endDate.ToString("yyyy-MM-dd HH:mm");
        Log($"[BTI] Filling start: {startVal}  end: {endVal}");

        await startInput.FillAsync(startVal, new() { Force = true });
        await endInput.FillAsync(endVal, new() { Force = true });

        // Debug: read back what actually ended up in the inputs
        var startActual = await startInput.InputValueAsync();
        var endActual   = await endInput.InputValueAsync();
        Log($"[BTI] Input values after fill — start: '{startActual}'  end: '{endActual}'");

        // ── Click filter SEARCH (nth(1) = filter button; nth(0) = header SEARCH) ──
        await page.Locator("button:has-text('Search')").Nth(1).ClickAsync();

        // After Search the filter panel collapses and "Expand Filter" reappears.
        // Because the panel was OPEN before we clicked Search, this text was NOT visible —
        // so WaitForAsync will only resolve once Search actually triggers the collapse,
        // meaning the bethistory API request is already in-flight.
        await page.Locator("text=Expand Filter").WaitForAsync(new() { Timeout = 15_000 });

        // Now NetworkIdle will fire *after* the bethistory response arrives
        // (not before, because the request is already in-flight at this point).
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 30_000 });

        // wait for a div with class total-block__Sum-sc-1shfc0w-0 cAOAxT to load
        Log("[BTI] Waiting for total count element to appear…");
        await page.Locator("div.total-block__Sum-sc-1shfc0w-0.cAOAxT").WaitForAsync(new() { Timeout = 30_000 });

        // ── Parse total count ──
        var totalText = await page.Locator("text=/Total: [\\d,]+/").First.TextContentAsync();
        Log($"[BTI] Raw total text: '{totalText}'");
        var total = int.Parse(Regex.Match(totalText!, "[\\d,]+").Value.Replace(",", ""));
        Log($"[BTI] Date range {startDate:yyyy-MM-dd HH:mm} → {endDate:yyyy-MM-dd HH:mm} — Total bets: {total}");

        if (total == 0) return [];

        // ── Set 500 records per page to reduce pagination round-trips ──
        await page.Locator("div:has-text('100 Records/Page')").Last.ClickAsync();
        await page.Locator("[role='option']:has-text('500 Records/Page')").ClickAsync();
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        await page.WaitForTimeoutAsync(2000);

        var totalPages = (int)Math.Ceiling((double)total / _perPage);
        var results = new List<BtiPurchaseDetail>(total);

        for (int pageNum = 1; pageNum <= totalPages; pageNum++)
        {
            Log($"[BTI] Scraping page {pageNum}/{totalPages}…");

            var pageResults = await ScrapeCurrentBetHistoryPageAsync(page);
            results.AddRange(pageResults);

            if (pageNum < totalPages)
                await NavigateToNextBetHistoryPageAsync(page, pageNum + 1);
        }

        Log($"[BTI] Done — {results.Count} bets collected.");
        return results;
    }

    private void Log(string message)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} {message}");
    }

    /// <summary>Reads all rows visible on the bet-history report table.</summary>
    private async Task<List<BtiPurchaseDetail>> ScrapeCurrentBetHistoryPageAsync(IPage page)
    {
        // Bet History table column indices (0-based):
        // 0=PurchaseId, 1=BetId, 2=BetType, 3=BetDate, 4=CustomerId, 5=CustomerLevel,
        // 6=CustomerTags, 7=Sport, 8=League, 9=Event, 12=Selection,
        // 18=Stake, 21=Operator, 23=MerchantCustomerCode, 26=WinLoss,
        // 28=ClientType, 31=BetStatus, 33=UserName, 36=SettledDate, 37=IsFreeBet

        // Wait for React to render at least the first row before we count.
        // Without this, CountAsync() returns 0 while DOM is still being built
        // (NetworkIdle fires after the API response, not after React finishes rendering 500 rows).
        try
        {
            await page.Locator("table tbody tr").First.WaitForAsync(new() { Timeout = 15_000 });
        }
        catch (TimeoutException)
        {
            Log("[BTI] Warning: no table rows found within 15 s — skipping page.");
            return [];
        }

        var rows = page.Locator("table tbody tr");
        var rowCount = await rows.CountAsync();
        Log($"[BTI]   → {rowCount} rows in DOM");
        var results = new List<BtiPurchaseDetail>(rowCount);

        var tasks = new List<Task>();

        for (int i = 0; i < rowCount; i++)
        {
            if(tasks.Where(t => !t.IsCompleted).Count() >= _batchSize)
            {
                Log($"[BTI] Too many concurrent tasks ({tasks.Count}) — waiting for some to complete…");
                await Task.WhenAll(tasks);
                tasks.RemoveAll(t => t.IsCompleted);
            }
            tasks.Add(ScrapeRowData(rows, results, i));
        }

        await Task.WhenAll(tasks);
        return results;
    }

    private  async Task ScrapeRowData(ILocator rows, List<BtiPurchaseDetail> results, int currentRow)
    {
        Log($"[BTI]     Start Scraping row {currentRow}…");
        var cells = rows.Nth(currentRow).Locator("td");
        var cellCount = await cells.CountAsync();
        // Skip header/footer rows that don't have enough data columns.
        // Use a low threshold (5) so we don't accidentally filter real data rows.
        if (cellCount < 5) return;
        // Skip rows that have no Purchase ID (blank spacer / group-header rows)
        var purchaseId = await GetCellTextAsync(cells, 0);
        if (string.IsNullOrWhiteSpace(purchaseId)) return;

        var data = new BtiPurchaseDetail
        {
            PurchaseId = await GetCellTextAsync(cells, 0),
            BetId = await GetCellTextAsync(cells, 1),
            BetType = await GetCellTextAsync(cells, 2),
            BetDate = await GetCellTextAsync(cells, 3),
            CustomerId = await GetCellTextAsync(cells, 4),
            Sport = await GetCellTextAsync(cells, 7),
            League = await GetCellTextAsync(cells, 8),
            Event = await GetCellTextAsync(cells, 9),
            Selection = await GetCellTextAsync(cells, 12),
            Stake = await GetCellTextAsync(cells, 18),
            Operator = await GetCellTextAsync(cells, 21),
            MerchantCustomerCode = await GetCellTextAsync(cells, 23),
            WinLoss = await GetCellTextAsync(cells, 26),
            BetStatus = await GetCellTextAsync(cells, 31),
            UserName = await GetCellTextAsync(cells, 33),
            SettledDate = await GetCellTextAsync(cells, 36),
            IsFreeBet = (await GetCellTextAsync(cells, 37))
                                      .Equals("Yes", StringComparison.OrdinalIgnoreCase),
        };

        await SemaphoreSlimLockAsync("append_results", async () =>
        {
            results.Add(data);
        });
        Log($"[BTI]     Finished scraping row {currentRow} Purchase ID: {purchaseId}");
    }

    private async Task SemaphoreSlimLockAsync(string key,Func<Task> action)
    {
        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            await action();
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <summary>
    /// Navigates to the next page using the "›" (next) button and waits for the table to refresh.
    /// Using the next button is more reliable than Go To Page + Enter, which doesn't always
    /// trigger the API call in this MUI layout.
    /// </summary>
    private static async Task NavigateToNextBetHistoryPageAsync(IPage page, int targetPage)
    {
        // Snapshot the first-row Purchase ID before navigating
        var firstRowIdBefore = await page.Locator("table tbody tr").First
            .Locator("td").First.TextContentAsync().ConfigureAwait(false);

        // All pagination buttons are MuiButton-outlined. The rightmost one is always "›" (next page).
        var outlinedBtns = page.Locator("button[class*='MuiButton-outlined']");
        var count = await outlinedBtns.CountAsync();
        await outlinedBtns.Nth(count - 1).ClickAsync();

        // Wait until the first row's Purchase ID changes — confirms the new page rendered
        await page.WaitForFunctionAsync(
            $"() => document.querySelector('table tbody tr td')?.textContent?.trim() !== '{firstRowIdBefore?.Trim()}'",
            null,
            new() { Timeout = 20_000 });

        // Extra buffer for the remainder of the 500 rows to finish rendering in React.
        // (WaitForFunction fires on the FIRST row change; the other 499 rows follow async.)
        await page.WaitForTimeoutAsync(1000);
    }

    private static async Task<string> GetCellTextAsync(ILocator cells, int index)
    {
        var cell = cells.Nth(index);
        if (await cell.CountAsync() == 0) return "";
        return (await cell.InnerTextAsync()).Trim();
    }
}
