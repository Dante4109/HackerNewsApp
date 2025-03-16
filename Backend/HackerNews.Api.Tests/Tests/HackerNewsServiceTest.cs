using Moq;
using Moq.Contrib.HttpClient;
using Microsoft.Extensions.Caching.Memory;
using HackerNews.Api.Services;
using Microsoft.Extensions.Options;
using HackerNews.Api.Tests.Mocks;


namespace HackerNews.Api.Tests.Tests
{
    public class HackerNewsServiceTests
    {
        // Arrange
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly HackerNewsService _hackerNewsService;
        private readonly IOptions<HackerNewsApiSettings> _hackerNewsApiSettings;


        public HackerNewsServiceTests() {
            
            // Initialize HackerNewsApiSettings
            _hackerNewsApiSettings = Options.Create(new HackerNewsApiSettings {
                NewestStoriesUrl = "https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty",
                StoryUrl = "https://hacker-news.firebaseio.com/v0/item/"
            });

            _memoryCache = new MemoryCache(new MemoryCacheOptions());

            // Create a mock handler using Moq.Contrib.HttpClient
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Setup mock response for the /newstories.json endpoint
            mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, _hackerNewsApiSettings.Value.NewestStoriesUrl)
                .ReturnsResponse("[1, 2, 3]"); // Returning a mock list of story IDs

            // Setup mock response for each individual story endpoint
            Enumerable.Range(1, 3).ToList().ForEach(i =>
                mockHttpMessageHandler
                    .SetupRequest(HttpMethod.Get, $"{_hackerNewsApiSettings.Value.StoryUrl}{i}.json?print=pretty")
                    .ReturnsResponse($"{{\"id\": {i}, \"title\": \"Story {i}\", \"url\": \"www.story{i}.com\"}}")
            );

            // Create HttpClient using the mock handler
            _httpClient = mockHttpMessageHandler.CreateClient();

            
            
            // Initialize HackerNewsService
            _hackerNewsService = new HackerNewsService(new HttpClientFactoryMock(_httpClient), _memoryCache, _hackerNewsApiSettings);
        }

        // Return Correct Stories Test
        [Fact]
        public async Task GetNewestStoriesAsync_Returns_Correct_Stories() {
            // Act

            // Get stories from hackerNewsService 
            var stories = await _hackerNewsService.GetNewestStoriesAsync();

            // Assert correct data is returned
            Assert.NotNull(stories);
            Assert.Equal(3, stories.Count);
            Assert.Equal("Story 1", stories[0].Title);
            Assert.Equal("Story 2", stories[1].Title);
            Assert.Equal("Story 3", stories[2].Title);
        }

        // Cache Hit Test
        [Fact]
        public async Task GetNewestStoriesAsync_CacheHit_Returns_Cached_Stories() {

            // Act

            // First call to load stories and populate the cache
            var storiesFirstCall = await _hackerNewsService.GetNewestStoriesAsync();

            // Change the data returned from HackerNews API to simulate their data has been updated            

            // Setup mock response for the /newstories.json endpoint            
            var mockHttpMessageHandlerSecondCall = new Mock<HttpMessageHandler>();

            // Create HttpClient using the second mock handler
            var httpClientSecondCall = mockHttpMessageHandlerSecondCall.CreateClient();

            // Setup mock response for the /newstories.json endpoint
            mockHttpMessageHandlerSecondCall
                .SetupRequest(HttpMethod.Get, _hackerNewsApiSettings.Value.NewestStoriesUrl)
                .ReturnsResponse("[4, 5, 6]"); // Returning a mock list of story IDs

            // Setup mock response for each individual story endpoint
            Enumerable.Range(4, 6).ToList().ForEach(i =>
                mockHttpMessageHandlerSecondCall
                    .SetupRequest(HttpMethod.Get, $"{_hackerNewsApiSettings.Value.StoryUrl}{ i}.json?print=pretty")
                    .ReturnsResponse($"{{\"id\": {i}, \"title\": \"Story {i}\", \"url\": \"www.story{i}.com\"}}")
            );

            // Second call to check if it hits the cache
            var storiesSecondCall = await _hackerNewsService.GetNewestStoriesAsync();

            // Assert that both calls return the same list of stories (cache hit)
            Assert.Same(storiesFirstCall, storiesSecondCall);
        }

        // Cache Miss Test
        [Fact]
        public async Task GetNewestStoriesAsync_CacheMiss_Fetches_From_API() {
            
            // Act
            
            // First call to load stories and populate the cache
            var storiesFirstCall = await _hackerNewsService.GetNewestStoriesAsync();

            // Change the data returned from HackerNews API to simulate their data has been updated

            // Setup mock response for the /newstories.json endpoint            
            var mockHttpMessageHandlerSecondCall = new Mock<HttpMessageHandler>();

            // Create HttpClient using the second mock handler
            var httpClientSecondCall = mockHttpMessageHandlerSecondCall.CreateClient();

            // Setup mock response for the /newstories.json endpoint
            mockHttpMessageHandlerSecondCall
                .SetupRequest(HttpMethod.Get, _hackerNewsApiSettings.Value.NewestStoriesUrl)
                .ReturnsResponse("[4, 5, 6]"); // Returning a mock list of story IDs

            // Setup mock response for each individual story endpoint
            Enumerable.Range(4, 6).ToList().ForEach(i =>
                mockHttpMessageHandlerSecondCall
                    .SetupRequest(HttpMethod.Get, $"{_hackerNewsApiSettings.Value.StoryUrl}{i}.json?print=pretty")
                    .ReturnsResponse($"{{\"id\": {i}, \"title\": \"Story {i}\", \"url\": \"www.story{i}.com\"}}")
            );

            // Clear the cache to simulate a cache miss
            _memoryCache.Remove("newestStories_default");

            // Second call to check if it hits the cache
            var storiesSecondCall = await _hackerNewsService.GetNewestStoriesAsync();

            // Assert that each all does not return the same stories (cache miss, fresh fetch)
            Assert.NotSame(storiesFirstCall, storiesSecondCall);
        }
    }
}