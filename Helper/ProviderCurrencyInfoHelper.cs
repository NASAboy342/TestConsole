using System;
using TestConsole.model;

namespace TestConsole.Helper;

public class ProviderCurrencyInfoHelper
{
    private readonly string _apiUrl = "https://arpia-uat.csmcex.com";
    public ProviderCurrencyInfoHelper(string apiUrl = null)
    {
        if (!string.IsNullOrEmpty(apiUrl))
        {
            _apiUrl = apiUrl;
        }
    }
    public async Task<ProviderCurrencyInfo> GetProviderCurrencyInfoAsync(int providerId)
    {
        var apiHelper = new HttpHelper();
        var getProviderCurrencyInfoRequest = new GetProviderCurrencyInfoRequest
        {
            IsGetForAllProvider = false,
            ProviderId = providerId,
            IsIncludeDisabled = false,
            IsReloadCache = false
        };
        var response = await apiHelper.PostAsync<GetProviderCurrencyInfoRequest, ProviderCurrencyInfoResponse>($"{_apiUrl}/api/Provider/GetProviderCurrencyInfo",getProviderCurrencyInfoRequest);    
        return response.Data;
    }
}
