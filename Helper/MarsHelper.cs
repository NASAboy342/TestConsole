using System.Text.Json;
using TestConsole.model;

namespace TestConsole.Helper;

public class MarsHelper
{
    private readonly string _apiUrl;
    private readonly string _companyKey;
    private readonly string _serverId;

    public MarsHelper(string apiUrl, string companyKey, string serverId)
    {
        _apiUrl = apiUrl;
        _companyKey = companyKey;
        _serverId = serverId;
    }

    public async Task<MarsGetGameListResponse> GetGameListAsync(int gpId, bool isGetAll = false)
    {
        var paramPayload = new
        {
            GpId = gpId,
            IsGetAll = isGetAll,
            CompanyKey = _companyKey,
            ServerId = _serverId
        };

        var serializedParam = JsonSerializer.Serialize(paramPayload);
        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(serializedParam), "Param");

        var requestUrl = $"{_apiUrl}/Api/GetGameList";
        Console.WriteLine($"Calling Mars GetGameList - Url: {requestUrl}, Param: {serializedParam}");

        using var httpClient = new HttpClient();
        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync(requestUrl, content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[EXCEPTION] Mars GetGameList call failed - Url: {requestUrl}, Exception: {ex.Message}");
            throw;
        }

        var statusCode = (int)response.StatusCode;
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[FAIL] Mars GetGameList - StatusCode: {statusCode}, Body: {responseBody}");
            response.EnsureSuccessStatusCode();
        }

        var result = JsonSerializer.Deserialize<MarsGetGameListResponse>(responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        Console.WriteLine($"[SUCCESS] Mars GetGameList - StatusCode: {statusCode}, GameCount: {result?.SeamlessGameProviderGames.Count ?? 0}");

        return result ?? new MarsGetGameListResponse();
    }
}
