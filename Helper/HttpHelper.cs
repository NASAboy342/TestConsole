using Newtonsoft.Json;
using System.Text;

namespace TestConsole.Helper
{
    public class HttpHelper
    {
        public async Task<string> PostAsync(string apiUrl, string serializedRequest)
        {
            try
            {

                var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                var response = await client.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode();

                var serializedResponse = await response.Content.ReadAsStringAsync();

                return serializedResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw;
            }
        }
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string apiUrl, TRequest request)
        {
            try
            {
                var serializedRequest = JsonConvert.SerializeObject(request);

                var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                var response = await client.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode();

                var serializedResponse = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TResponse>(serializedResponse);

                return responseData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw;
            }
        }
        public async Task<TResponse> PostAsync<TRequest, TResponse>(string apiUrl, TRequest request, Dictionary<string, string> header)
        {
            try
            {
                var serializedRequest = JsonConvert.SerializeObject(request);

                var content = new StringContent(serializedRequest, Encoding.UTF8, "application/json");
                using var client = new HttpClient();
                foreach (var item in header)
                {
                    client.DefaultRequestHeaders.Add(item.Key, item.Value);
                }

                var response = await client.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode();

                var serializedResponse = await response.Content.ReadAsStringAsync();
                var responseData = JsonConvert.DeserializeObject<TResponse>(serializedResponse);

                return responseData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
                throw;
            }
        }
    }
}