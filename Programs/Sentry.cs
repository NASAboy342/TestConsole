using System;
using System.Security.Cryptography;
using System.Text;
using MathNet.Numerics.Random;
using Newtonsoft.Json;
using TestConsole.Helper;

namespace TestConsole.Programs;

public class Sentry
{
    public async Task NormalTest()
    {
        VaultResponse response = await GetAgentInfo();
        Console.WriteLine(JsonConvert.SerializeObject(response));
    }

    public async Task StressTest()
    {
        var tasks = new List<Task<VaultResponse>>();
        var startTime = DateTime.Now;
        var requestCount = 100;
        for (int i = 0; i < requestCount; i++)
        {
            tasks.Add(Task.Run(async () => await GetAgentInfo(GetRandomGpId(), "")));
        }
        var finishCallTime = DateTime.Now;
        Console.WriteLine($"Start: {startTime}, End: {finishCallTime}, Finish calling at Duration: {(finishCallTime - startTime).TotalMilliseconds} ms");
        var results = await Task.WhenAll(tasks);
        var endTime = DateTime.Now;
        Console.WriteLine($"Start: {startTime}, End: {endTime}, Finish response at Duration: {(endTime - startTime).TotalSeconds}s  { requestCount / ((endTime - startTime).TotalSeconds)}requests/s");
        Console.WriteLine($"Total responses: {results.Length} ===============================");
        foreach (var result in results)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new { result.errorCode, result.errorMessage, Count = result.result?.Count ?? 0 }));
        }
    }

    private int GetRandomGpId()
    {
        var gpids = new List<int>
        {
            0,2,3,5,7,10,11,12,13,14,15,16,18,19,20,22,28,29,30,33,35,36,37,38,39,40,41,42,43,44,45,46,47,49,50,51,52,53,101,1000,1009,1010,1011,1012,1013,1015,1016,1017,1018,1019,1020,1021,1022,1023,1024,1025,1026,1027,1028,1029,1029,1029,1029,1029,1029,1029,1029,1029,1030,1031,1032,1033,1034,1035,1036,1037,1038,1038,1039,1040,1041,1042,1043,1044,1045,1046,1047,1048,1049,1050,1051,1052,1053,1054,1055,1056,1057,1058,1059,1060,1061,1062,1063,1064,1065,1066,1067,1068,1069,1070,1071,1072,1073,1074,1075,1076,1077,1078,1079,1080,1081,1082,1083,1084,1085,1086,1087,1088,1089,1090
        };
        var randomIndex = new Random().Next(0, gpids.Count - 1);
        var gpid = gpids[randomIndex];
        return gpid;
    }

    private static async Task<VaultResponse> GetAgentInfo(int gpid = 0, string providerKey = "")
    {
        var header = new Dictionary<string, string>
        {
            {"X-API-Key", String.IsNullOrEmpty(providerKey)? "123456789qwertyui" : providerKey},
            {"X-Signature", ""},
            {"X-Timestamp", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")},
        };
        var body = new VaultRequest
        {
            ProviderId = gpid == 0 ? 3 : gpid,
            Environment = "UAT",
            ProjectName = "Pin demo console stress-testing",
            RequestTime = DateTime.UtcNow.ToString()
        };

        header["X-Signature"] = ComputeHmacSHA256(JsonConvert.SerializeObject(body), header["X-API-Key"]);

        var http = new HttpHelper();
        var demoUrl = "https://sentry-uat.568winex.com";
        var localUrl = "http://localhost:5218";
        Console.WriteLine($"Request: {JsonConvert.SerializeObject(body)}");
        var response = await http.PostAsync<VaultRequest, VaultResponse>($"{demoUrl}/api/Vault/vault-request", body, header);
        Console.WriteLine($"Response: {JsonConvert.SerializeObject(response)}");
        return response;
    }

    public static string GenerateHmac(string data, string secretKey)
    {
        if (string.IsNullOrEmpty(data))
            throw new ArgumentException("Data cannot be null or empty", nameof(data));

        if (string.IsNullOrEmpty(secretKey))
            throw new ArgumentException("Secret key cannot be null or empty", nameof(secretKey));

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return Convert.ToBase64String(hashBytes);
    }
    public static string ComputeHmacSHA256(string input, string secretKey)
    {
        // Convert input and key to byte arrays
        byte[] keyBytes = Encoding.UTF8.GetBytes(secretKey);
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        // Compute the HMAC
        using (var hmacSha256 = new HMACSHA256(keyBytes))
        {
            byte[] hashBytes = hmacSha256.ComputeHash(inputBytes);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}

public class VaultRequest
{
    public int ProviderId { get; set; }
    public string Environment { get; set; }
    public string ProjectName { get; set; }
    public string RequestTime { get; set; }
}

public class VaultResponse
{
    public int errorCode { get; set; }
    public string errorMessage { get; set; }
    public List<VaultResult> result { get; set; }
}

public class VaultResult
{
    public int errorCode { get; set; }
    public string errorMessage { get; set; }
    public int providerId { get; set; }
    public string currency { get; set; }
    public string apiUrl { get; set; }
    public string apiUrl1 { get; set; }
    public string agentId { get; set; }
    public string agentName { get; set; }
    public string cert { get; set; }
    public string cert1 { get; set; }
    public int webId { get; set; }
}