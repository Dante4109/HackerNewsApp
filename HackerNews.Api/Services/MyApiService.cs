using Newtonsoft.Json;

namespace HackerNews.Api.Services
{
    public class MyApiService
    {
        private readonly HttpClient _httpClient;

        public MyApiService(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public async Task<string> GetDataAsync() {
            var response = await _httpClient.GetAsync("https://catfact.ninja/fact");                        
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
    }

    public class CatFact
    {
        public string? Fact { get; set; }
        public int? Length { get; set; }
    }
}
