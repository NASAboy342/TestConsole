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

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent(JsonSerializer.Serialize(paramPayload)), "Param");

        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync($"{_apiUrl}/Api/GetGameList", content);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<MarsGetGameListResponse>(responseBody,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result ?? new MarsGetGameListResponse();
    }
}
