
using System.Text;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main()
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("http://mapi-uat.remotes.local") };
        var api = new SportsEventApi(httpClient);

        try
        {
            var result = await api.GetSportsEventsAsync(
                1080, "2025-10-01T00:00:00Z", "2025-10-30T23:59:59Z");
            Console.WriteLine($"Found {result.Count} events:");
            foreach (var ev in result.Events)
            {
                Console.WriteLine($"{ev.EventName} ({ev.StartEventDate})");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to get sports events: {ex.Message}");
        }
    }
}


public class SportsEventResult
{
    public int Count { get; set; }
    public int ProviderId { get; set; }
    public Event[] Events { get; set; }
}

public class Event
{
    public string Id { get; set; }
    public string EventName { get; set; }
    public string StartEventDate { get; set; }
    public string Status { get; set; }
    public int TotalActiveMarketsCount { get; set; }
    public bool IsLive { get; set; }
    public bool IsSuspended { get; set; }
    public bool IsTopLeague { get; set; }
    public string EventType { get; set; }
    public string SportName { get; set; }
    public string LeagueName { get; set; }
    public string RegionCode { get; set; }
    public string RegionName { get; set; }
    public string HomeTeamName { get; set; }
    public string AwayTeamName { get; set; }
}

public class ApiResponse
{
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public SportsEventResult Result { get; set; }
}

public class SportsEventApi
{
    private readonly HttpClient _httpClient;

    public SportsEventApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<SportsEventResult> GetSportsEventsAsync(
        int providerId, string startDate = null, string endDate = null)
    {
        var payload = new
        {
            providerId,
            startDate,
            endDate
        };

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("/Api/GetSportsEvents", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

        if (apiResponse.ErrorCode == 0)
        {
            return apiResponse.Result;
        }
        else
        {
            throw new Exception(apiResponse.ErrorMessage);
        }
    }
}
