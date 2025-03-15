using Moq;
using Moq.Contrib.HttpClient;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using HackerNews.Api.Services;

namespace HackerNews.Api.Tests
{
    public class HackerNewsServiceTests
    {
        // Arrange
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly HackerNewsService _hackerNewsService;

        public HackerNewsServiceTests() {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Create a mock HttpClient using Moq.Contrib.HttpClient
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Setup mock response for the /newstories.json endpoint
            mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, "https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty")
                .ReturnsResponse("[1, 2, 3]"); // Returning a mock list of story IDs

            // Setup mock response for each individual story endpoint
            mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, "https://hacker-news.firebaseio.com/v0/item/1.json?print=pretty")
                .ReturnsResponse("{\"id\": 1, \"title\": \"Story 1\", \"url\": \"www.story1.com\"}");
            mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, "https://hacker-news.firebaseio.com/v0/item/2.json?print=pretty")
                .ReturnsResponse("{\"id\": 2, \"title\": \"Story 2\", \"url\": \"www.story2.com\"}");
            mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, "https://hacker-news.firebaseio.com/v0/item/3.json?print=pretty")
                .ReturnsResponse("{\"id\": 3, \"title\": \"Story 3\", \"url\": \"www.story3.com\"}");

            // Create HttpClient using the mock handler
            _httpClient = mockHttpMessageHandler.CreateClient();

            // Initialize HackerNewsService
            _hackerNewsService = new HackerNewsService(new HttpClientFactoryMock(_httpClient), _memoryCache);
        }

        [Fact]
        public async Task GetNewestStoriesAsync_Returns_Correct_Stories() {
            // Act
            var stories = await _hackerNewsService.GetNewestStoriesAsync();

            // Assert
            Assert.NotNull(stories);
            Assert.Equal(3, stories.Count);
            Assert.Equal("Story 1", stories[0].Title);
            Assert.Equal("Story 2", stories[1].Title);
            Assert.Equal("Story 3", stories[2].Title);
        }
    }

    // Mock IHttpClientFactory implementation
    public class HttpClientFactoryMock : IHttpClientFactory
    {
        private readonly HttpClient _httpClient;

        public HttpClientFactoryMock(HttpClient httpClient) {
            _httpClient = httpClient;
        }

        public HttpClient CreateClient(string name = null) {
            return _httpClient;
        }
    }
}