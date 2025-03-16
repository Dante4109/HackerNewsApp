namespace HackerNews.Api.Tests.Mocks
{
    // Mock IHttpClientFactory implementation
    public class HttpClientFactoryMock : IHttpClientFactory
    {

        // The HttpClient instance to be returned by the factory
        private readonly HttpClient _httpClient;

        // Constructor that takes an HttpClient instance
        public HttpClientFactoryMock(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        // CreateClient method that returns the HttpClient instance
        public HttpClient CreateClient(string name = null) {
            return _httpClient;
        }
    }
}