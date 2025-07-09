using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Presentation;
using IEnumerable.ForEach;
using LibGit2Sharp;
using MiNET.UI;
using Newtonsoft.Json;
using NPOI.HPSF;
using NPOI.SS.Formula.Functions;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Org.BouncyCastle.Asn1.Mozilla;
using Org.BouncyCastle.Crmf;
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Web;
using TestConsole;
using TestConsole.Helper;
using TestConsole.Programs;
using TestConsole.Tests;

internal class Program
{
    private static async Task Main(string[] args)
    {
        // Create Edge driver
        var options = new EdgeOptions();
        using var driver = new EdgeDriver(options);

        try
        {
            // 1. Navigate to Google
            driver.Navigate().GoToUrl("https://www.google.com");

            await Task.Delay(TimeSpan.FromSeconds(5));

            // 2. Accept cookies if the popup shows (optional, Google often shows one)
            try
            {
                var agreeButton = driver.FindElement(By.XPath("//button[contains(text(),'Accept all')]"));
                agreeButton.Click();
            }
            catch (NoSuchElementException) { /* no popup shown */ }

            await Task.Delay(TimeSpan.FromSeconds(5));

            // 3. Search for "next Christmas day"
            var searchBox = driver.FindElement(By.Name("q"));
            searchBox.SendKeys("next Christmas day");
            await Task.Delay(TimeSpan.FromSeconds(2));
            searchBox.SendKeys(Keys.Enter);

            // 4. Wait for the result
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var result = wait.Until(d =>
                d.FindElement(By.CssSelector("div[data-attrid='wa:/description']")) ??
                d.FindElement(By.CssSelector("div[data-attrid='kc:/time/holiday:date']")) ??
                d.FindElement(By.CssSelector("div[jsname='W297wb']"))
            );

            // 5. Output the result
            Console.WriteLine("Top result: " + result.Text);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Something went wrong: " + ex.Message);
        }
        finally
        {
            // Close browser after short delay
            System.Threading.Thread.Sleep(5000);
            driver.Quit();
        }
    }
    //var exform = GetExcelToSqlValueStatement(new List<ExcelColumn>
    //{
    //    new ExcelColumn(){DataType = EnumSqlDateType.Int, Name = "A"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "B"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "C"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "D"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "E"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "F"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "G"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "H"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "I"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.DateTime, Name = "J"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "K"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.DateTime, Name = "L"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "M"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Int, Name = "N"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Int, Name = "O"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "P"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "Q"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.DateTime, Name = "R"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "S"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "T"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "U"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "V"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "W"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "X"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "Y"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "Z"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AA"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AB"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AC"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AD"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AE"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AF"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AG"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AH"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AI"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AJ"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AK"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AL"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AM"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AN"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AO"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AP"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AQ"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AR"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AS"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AT"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AU"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AV"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AW"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AX"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "AY"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Bit, Name = "AZ"},
    //    new ExcelColumn(){DataType = EnumSqlDateType.Nvarchar, Name = "BA"}
    //});

    //Write(exform);
    private static async Task<string> GetMarsPublicTopDomian()
    {
        var marsPublicDomain = "lmd.xijiangx.com";
        var parts = marsPublicDomain.Split('.');
        var topLevelDomain = string.Join(".", parts[^2], parts[^1]);
        return topLevelDomain;
    }
    public static long GetTimeStamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private static object ConvertTo2DecimalPoint(decimal amount)
    {
        return Convert.ToDecimal((Math.Truncate(amount * 100m) / 100m).ToString("0.00"));
    }

    private static string FormatElement(JsonElement element, string prefix = "")
    {
        string result = "";
        foreach (JsonProperty prop in element.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.Object)
            {
                result += FormatElement(prop.Value, prefix + prop.Name + ".");
            }
            else if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                result += $"{prefix}{prop.Name} = {prop.Value},";
            }
            else
            {
                result += $"{prefix}{prop.Name} = {prop.Value},";
            }
        }
        return result;
    }
    public static decimal CalculatePayoutAmericanNegativeOdds(decimal stake, decimal negativeOdds)
    {
        if (negativeOdds >= 0)
            throw new ArgumentException("Odds must be negative for this calculation.");

        decimal profit = stake / (Math.Abs(negativeOdds) / 100m);

        return profit;
    }
    private static string GetSatus(string status)
    {
        return (status switch
        {
            "WON" => SportStatusEnum.Won,
            "LOSE" => SportStatusEnum.Lose,
            "DRAW" => SportStatusEnum.Draw,
            "HALF_WON_HALF_PUSHED" => SportStatusEnum.HalfWon,
            "HALF_LOST_HALF_PUSHED" => SportStatusEnum.HalfLost,
            _ => SportStatusEnum.Running
        }).GetDescription() ?? string.Empty;
    }
    private static DateTime ConvertTimestampToDateTime(string timestamp)
    {
        var timestampMilliseconds = long.Parse(timestamp);
        var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(timestampMilliseconds);
        return dateTimeOffset.UtcDateTime.AddHours(-4);
    }
    public static List<KeyValuePair<string, string>> ToKeyValuePairs(object obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var keyValuePairs = new List<KeyValuePair<string, string>>();

        foreach (PropertyInfo prop in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            object value = prop.GetValue(obj);
            keyValuePairs.Add(new KeyValuePair<string, string>(prop.Name, value?.ToString() ?? string.Empty));
        }

        return keyValuePairs;
    }

    public static string Encrypt(string text, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // Initialize IV to zero (you can use a random IV and store it along with the encrypted data)

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(text);
                    }
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public static string DDecrypt(string encryptedText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[16]; // Initialize IV to zero (make sure to use the same IV used for encryption)

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(encryptedText)))
            {
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader sr = new StreamReader(cs))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }
    }


    public static Dictionary<string, object> ToDictionary(object obj)
    {
        return obj.GetType()
                  .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                  .ToDictionary(
                      prop => prop.GetCustomAttribute<JsonPropertyAttribute>()?.PropertyName ?? prop.Name,
                      prop => prop.GetValue(obj)
                  );
    }

    public static string GenerateSign(Dictionary<string, object> data, string apiKey)
    {
        var sortedData = data.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);
        //var queryString = string.Join("&", sortedData.Select(pair => $"{WebUtility.UrlEncode(pair.Key)}={WebUtility.UrlEncode(pair.Value.ToString()).ToUpper()}"));
        var queryString = "end_time=2020-07-02T00%3A00%3A00.000%2B08%3A00=1&page_size=2000&start_time=2020-07-01T00%3A00%3A00.000%2B08%3A00";
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(queryString + apiKey));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
        //return ComputeMd5HashToLowerCase(queryString + apiKey);
    }
    public static string ComputeMd5HashToLowerCase(string input)
    {
        using (var md5 = MD5.Create())
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var t in hashBytes)
            {
                sb.Append(t.ToString("x2"));
            }
            return sb.ToString();
        }
    }

    private static void GetGitHistory()
    {
        var allcommits = new List<CustCommit>();
        using (var repo = new Repository(@"D:\Clones\reportia"))
        {
            var branches = repo.Branches
                .Where(b => b.FriendlyName.StartsWith("origin") && !b.FriendlyName.StartsWith("origin/HEAD"))
                .OrderBy(b => repo.ObjectDatabase.FindMergeBase(b.Tip, repo.Head.Tip).Committer.When)
                .ToList();

            foreach (var branch in branches)
            {
                allcommits.AddRange(branch.Commits
                    .Where(commit => commit.Committer.When > DateTime.Now.AddDays(-7))
                    .Select(commit => new CustCommit
                    {
                        Hash = commit.Sha,
                        BranchName = branch.FriendlyName.Replace("origin/", ""),
                        Message = commit.MessageShort,
                        CommitTime = commit.Committer.When,
                        IsAMergeCommit = commit.Parents.Count() > 1,
                        MergeFrom = commit.Parents.Count() > 1 ? commit.Parents.ToList()[1].Sha : "",
                    })
                    .ToList());
            }

            allcommits = allcommits
                .OrderByDescending(c => c.CommitTime)
                .GroupBy(c => c.Hash)
                .Select(c => new CustCommit
                {
                    Hash = c.Key,
                    BranchName = JsonConvert.SerializeObject(c.Select(cc => cc.BranchName)),
                    Message = c.Last().Message,
                    CommitTime = c.Last().CommitTime,
                    IsAMergeCommit = c.Last().IsAMergeCommit,
                    MergeFrom = c.Last().MergeFrom,
                })
                .ToList();
        }
        Write(JsonConvert.SerializeObject(allcommits));
    }

    private static string Decrypt(string encrypted, string key)
    {
        var des = new DESCryptoServiceProvider();

        var rfc2898 = new Rfc2898DeriveBytes(key, HashByMD5(key));

        des.Key = rfc2898.GetBytes(des.KeySize / 8);
        des.IV = rfc2898.GetBytes(des.BlockSize / 8);

        var dateByteArray = Convert.FromBase64String(encrypted);

        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(dateByteArray, 0, dateByteArray.Length);
        cs.FlushFinalBlock();

        return Encoding.UTF8.GetString(ms.ToArray());
    }
    private static byte[] HashByMD5(string source)
    {
        return MD5.HashData(Encoding.UTF8.GetBytes(source));
    }
    private static decimal GetSpecialCurrencyValue(string currency, string provider)
    {
        if (provider.Contains("_SBOBET") && currency.Equals("VND", StringComparison.OrdinalIgnoreCase)) return 1;
        if (currency.Equals("VND", StringComparison.OrdinalIgnoreCase) || currency.Equals("IDR", StringComparison.OrdinalIgnoreCase)) return 1000;
        return 1;
    }

    private static string GetExcelToSqlValueStatement(List<ExcelColumn> columns, int atRow = 2)
    {
        var sqlStatement = new StringBuilder();
        sqlStatement.Append("(");
        foreach (var column in columns)
        {
            switch (column.DataType)
            {
                case EnumSqlDateType.Int:
                case EnumSqlDateType.Decimal:
                    sqlStatement.Append($"\"&{column.Name}{atRow}&\",");
                    break;
                case EnumSqlDateType.DateTime:
                    sqlStatement.Append($"'\"&TEXT({column.Name}{atRow},\"yyyy-mm-dd hh:mm:ss\")&\"',");
                    break;
                case EnumSqlDateType.Nvarchar:
                    sqlStatement.Append($"N'\"&{column.Name}{atRow}&\"',");
                    break;
                case EnumSqlDateType.Varchar:
                    sqlStatement.Append($"'\"&{column.Name}{atRow}&\"',");
                    break;
                case EnumSqlDateType.Bit:
                    sqlStatement.Append($"\"&IF({column.Name}{atRow}=\"TRUE\", 1, 0)&\",");
                    break;
            }
        }
        sqlStatement.Remove(sqlStatement.Length - 1, 1).Append("),");
        return sqlStatement.ToString();
    }
    private static string GetCurrencyRemarks(decimal ExchangeRate)
    {
        if (ExchangeRate > 1)
        {
            return $"Provider uses 1:1, customer uses 1:{ExchangeRate}";
        }
        else if (ExchangeRate < 1)
        {
            return $"Provider uses 1:{1 / ExchangeRate}, customer uses 1:1";
        }
        else
        {
            return "";
        }
    }
    public static async Task<List<string>> GetRandomStringAsyn()
    {
        var startTime = DateTime.Now;
        while (DateTime.Now - startTime <= TimeSpan.FromSeconds(3))
        {
            Console.WriteLine($"{DateTime.Now - startTime}");
        }
        var random = new Random();
        var st = random.Next(1, 100); 
        return new List<string> { $"String {st}" };
    }
    private static string GetRandomString()
    {
        var random = new Random();
        var st = random.Next(1, 100);
        var startTime = DateTime.Now;
        while(DateTime.Now - startTime < TimeSpan.FromSeconds(1))
        {

        }
        return $"String {st}";
    }

    private static async Task<string> DoSomeing()
    {
        Console.WriteLine("Main method started.");

        // Start a background task
        var tast = Task.Run(() => ExecuteBackgroundTask());

        // Continue with other work
        Console.WriteLine("Main method continuing...");

        // Wait for the background task to complete (if needed)

        Console.WriteLine("Main method completed.");
        return "Completed";
    }

    private static async Task ExecuteBackgroundTask()
    {
        // Simulate some work
        Console.WriteLine("Background task started.");
        await Task.Delay(5000); // Simulating work (e.g., database call, API request)
        throw new Exception();
        Console.WriteLine("Background task completed.");
    }

    public static DateTime ConvertTimeFromAndTo(DateTime dateTime, int fromTimeZone, int toTimeZone)
    {
        var utcTime = dateTime.AddHours(-fromTimeZone);
        var convertedTime = utcTime.AddHours(toTimeZone);
        return convertedTime;
    }

    public string GetKey(string customerName)
    {
        return $"C-R-S-F-P-{customerName}";
    }

    public string GetCustomerName(string key)
    {
        return key.Substring(10);
    }

    public static DateTime ConvertToTimeZone(DateTime inputDateTime, int targetTimeZoneOffset)
    {
        // Get the local timezone
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;

        // Calculate the target timezone offset in minutes
        int targetOffsetMinutes = targetTimeZoneOffset * 60;

        // Create a TimeSpan with the target offset
        TimeSpan targetOffset = TimeSpan.FromMinutes(targetOffsetMinutes);

        // Convert the inputDateTime to UTC (since TimeZoneInfo works with UTC)
        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(inputDateTime, localTimeZone);

        // Add the target offset to the UTC time
        DateTime convertedDateTime = utcDateTime + targetOffset;

        return convertedDateTime;
    }

    public static void Write(object text)
    {
        if (text.GetType() == typeof(string))
        {
            Console.WriteLine(text);
        }
        else
        {
            Console.WriteLine(JsonConvert.SerializeObject(text));
        }
    }

    private static string FormatCustomerSupportCurrency(string customerSupportCurrency)
    {
        var currencies = customerSupportCurrency?
            .Split(',')
            .Where(c => c.Length > 0)
            .Order()
            .ToList() ?? new List<string>();

        return JsonConvert.SerializeObject(currencies)
            .Replace("[", "")
            .Replace("]", "")
            .Replace("\"", "")
            .Replace(" ", "");
    }

    public static Dictionary<string, string> ToParameterDictionary<T>(T parameters, bool isCamelCase) where T : class
    {
        var enumerable = parameters.GetType()
            .GetProperties()
            .Where(p => p.GetValue(parameters, null) != null &&
                        p.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Length == 0);
        var dictionary = new Dictionary<string, string>();
        enumerable.ForEach(p =>
            dictionary.Add(isCamelCase ? (char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1)) : p.Name,
                GetParameterValue(parameters, p).ToString()));
        return dictionary;
    }

    private static string GetParameterValue<T>(T parameters, PropertyInfo property) where T : class
    {
        var value = property.GetValue(parameters, null);
        var result = value.ToString();

        return result;
    }

    public static IEnumerable<ActiveUserByCurrencyResult> ProcessActiveUsers(IEnumerable<ActiveUserByCurrencyResponse> data)
    {
        return data
            .GroupBy(d => new { d.Customer, d.BrandName, d.Currency })
            .Select(g => new ActiveUserByCurrencyResult
            {
                ActiveUser = g.Select(x => x.AccountId).Distinct().Count(),
                Customer = string.IsNullOrEmpty(g.Key.BrandName) ? g.Key.Customer : g.Key.BrandName,
                Currency = g.Key.Currency
            });
    }

    public static DateTime GetFirstDayOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    public static DateTime GetLastDayOfMonth(DateTime date)
    {
        return GetFirstDayOfMonth(date).AddMonths(1).AddDays(-1);
    }

    private static async Task<string> GetMarsPublicTopDomain()
    {
        var marsPublicDomain = "lmd-uat.gaolitsai.com";
        var topLevelDomain = marsPublicDomain.Substring(marsPublicDomain.LastIndexOf('.') + 1);
        return topLevelDomain;
    }
}

public class ActiveUserByCurrencyResult
{
    public int ActiveUser { get; set; }
    public string Customer { get; set; }
    public string Currency { get; set; }
}

public class ActiveUserByCurrencyResponse
{
    public DateTime? RecordDate { get; set; }
    public string? Customer { get; set; }
    public string? ProductType { get; set; }
    public string Currency { get; set; }
    public int? WebId { get; set; }
    public string? BrandName { get; set; }
    public string? AccountId { get; set; }
}

public class GameSession
{
    public string GameCode { get; set; }
    public string UserName { get; set; }
    public string PlayerId { get; set; }
    public string Currency { get; set; }
    public string Language { get; set; }
    public string PlayerIp { get; set; }
    public string SessionId { get; set; }
    public bool IsTestAccount { get; set; }
}

public class PercentageSettings
{
    public int Percentage { get; set; }
    public bool IsEnabled { get; set; }
}

public class MonthlyReportRequest
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}

public class CurrencyExchangeRate
{
    public string Currency { get; set; }
    public decimal ExchangeRate { get; set; }
}

public enum Enum568WinSubProvider
{
    _568winGames,
    AFBGaming,
    Tenda,
    Rival,
    OnlyPlay,
    Revolver,
    Aeon,
    KhGames,
    Anthony
}
public enum EnumSqlDateType
{
    DateTime,
    Int,
    Nvarchar,
    Varchar,
    Decimal,
    Bit,
}
public class ExcelColumn
{
    public EnumSqlDateType DataType { get; set; }
    public string Name { get; set; }
}

public class DisposableClass : IDisposable
{
    public DisposableClass(){
        Console.WriteLine("class start");
    }
    public void Dispose()
    {
        Console.WriteLine("class end");
    }
}

public class CustCommit
{
    public string Hash { get; set; }
    public DateTimeOffset CommitTime { get; set; }
    public string Message { get; set; }
    public string BranchName { get; set; }
    public bool IsAMergeCommit { get; set; }
    public string MergeFrom { get; set; } = "";
    public string Id { get; set; } = "";
}
public class SampleObject
{
    [JsonProperty("start_time")]
    public string StartTime { get; set; }

    [JsonProperty("end_time")]
    public string EndTime { get; set; }

    [JsonProperty("page_size")]
    public int? PageSize { get; set; } = 0;

    [JsonProperty("page")]
    public int Page { get; set; }
}
public enum SportStatusEnum
{
    [Description("Draw")]
    Draw,
    [Description("Lose")]
    Lose,
    [Description("Refund")]
    Refund,
    [Description("Running")]
    Running,
    [Description("Void")]
    Void,
    [Description("Waiting")]
    Waiting,
    [Description("Won")]
    Won,
    [Description("Half Won")]
    HalfWon,
    [Description("Half Lost")]
    HalfLost,
    [Description("Waiting Reject")]
    WaitingReject,
    [Description("Cashout")]
    Cashout,
}
public static class DescriptionExtension
{
    public static string GetDescription<T>(this T enumParameter)
    {
        var type = enumParameter.GetType();
        if (!type.IsEnum) throw new ArgumentException("EnumParameter is not the type of Enum", nameof(enumParameter));

        var memberInfo = type.GetMember(enumParameter.ToString());
        if (memberInfo.Length <= 0) return enumParameter.ToString();

        var attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attrs.Length > 0 ? ((DescriptionAttribute)attrs[0]).Description : enumParameter.ToString();
    }
}