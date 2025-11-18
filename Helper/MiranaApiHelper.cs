using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestConsole.model;

namespace TestConsole.Helper;

public class MiranaApiHelper
{
    private readonly Dictionary<string, string> EnvironmentUrls = new Dictionary<string, string>
    {
        {"demo", "https://ex-api-demo-yy.568win.com"},
        {"yy", "https://ex-api-yy.568win.com"},
        {"yy2", "https://ex-api-yy2.568win.com"},
        {"yy4", "https://ex-api-yy4.568win.com"},
        {"yy5", "https://ex-api-yy5.568win.com"},
        {"yy7", "https://ex-api-yy7.568win.com"},
        {"dr_yy1", "https://ex-api-dr-yy1.568win.com"},
        {"dr_yy2", "https://ex-api-dr-yy2.568win.com"},
        {"dr_yy4", "https://ex-api-dr-yy4.568win.com"},
        {"dr_yy5", "https://ex-api-dr-yy5.568win.com"},
    };

    internal async Task<MiranaGamelist?> FetchGameListAsync(string gpidInput, string env)
    {
        using var client = new HttpClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{EnvironmentUrls[env]}/web-root/restricted/information/get-game-list.aspx");
        request.Headers.Add("Cookie", "__cf_bm=1.AKH2dLda1NZMocjLfivwU.QEznRmj7SC_SI5QekPw-1763434984.9970896-1.0.1.1-_0MeVoMxWNpej1hsr5RhsHteMwEcppBkaPaYZUIRc7qpbPWmTp4LR.hjj1BkT6SzDOFsvDWTzjXTS6DEHYZt5h7fAbGOjhVdTNOCE.HMA7UAppk76klSbJS97PA8Aojw");

        var body = new GetGameListRequest
        {
            GpId = int.TryParse(gpidInput, out var gp) ? gp : throw new ArgumentException("Invalid GPID input"),
            IsGetAll = false,
            CompanyKey = "",
            ServerId = ""
        };

        var content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
        request.Content = content;
        Console.WriteLine($"Request to {EnvironmentUrls[env]}/web-root/restricted/information/get-game-list.aspx Body: {JsonSerializer.Serialize(body)}");
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response: {responseString}");

        var result = JsonSerializer.Deserialize<MiranaGamelist>(responseString);
        return result;
    }
}

public class GetGameListRequest
{
  public int GpId { get; set; }
  public bool IsGetAll { get; set; }
  public string CompanyKey { get; set; } = "";
  public string ServerId { get; set; } = "";
}