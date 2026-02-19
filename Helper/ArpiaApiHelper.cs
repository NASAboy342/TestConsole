using System;
using TestConsole.model;

namespace TestConsole.Helper;

public class ArpiaApiHelper
{
    private readonly string _baseUrl = "https://arpia-uat.csmcex.com";
    public ArpiaApiHelper(string baseUrl = "")
    {
        if (!string.IsNullOrEmpty(baseUrl)) _baseUrl = baseUrl;
    }

    public async Task<ArpiaGetAllGameResponse> GetAllGamesAsync(int providerId)
    {
        var req = new ArpiaGetAllGameRequest
        {
            ProviderId = providerId
        };
        var httpHelper = new HttpHelper();
        var response = await httpHelper.PostAsync<ArpiaGetAllGameRequest, ArpiaGetAllGameResponse>($"{_baseUrl}/api/Game/GetAll", req);
        return response;
    }
}
