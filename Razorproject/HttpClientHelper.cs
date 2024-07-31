
namespace Razorproject
{
    public static class HttpClientHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<HttpResponseMessage> PostAsync(string url, object data)
        {
            return await _httpClient.PostAsJsonAsync(url, data);
        }
    }

}
