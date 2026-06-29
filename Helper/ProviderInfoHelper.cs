using System;
using TestConsole.model;

namespace TestConsole.Helper;

public class ProviderInfoHelper
{
    public async Task<List<ProviderInfoData>> GetProviderInfos(string url = "http://lcg-mars-d10.568winex.com", int gpid = 0, bool isGetAll = false)
    {
        var request = new
        {
            GpId = gpid,
            IsGetAll = isGetAll
        };
        var apiHelper = new HttpHelper();
        var response = await apiHelper.PostAsync<object, GetProviderInfoResponse>(url + "/Api/GetAllProviderInfo", request);

        return response.Data;
    }
}
