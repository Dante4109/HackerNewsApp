using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HackerNews.Api.Services
{
    // Service to interact with Hacker News API and retrieve stories
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly string _newestStoriesUrl;
        private readonly string _storyUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        // Constructor injecting IHttpClientFactory, IMemoryCache, and ApiSettings for Hacker News API
        public HackerNewsService(IHttpClientFactory httpClientFactory, IMemoryCache cache, IOptions<HackerNewsApiSettings> hackerNewsApiSettings
            ) {
            _httpClientFactory = httpClientFactory;
            _memoryCache = cache;
            _newestStoriesUrl = hackerNewsApiSettings.Value.NewestStoriesUrl;
            _storyUrl = hackerNewsApiSettings.Value.StoryUrl;
        }

        public async Task<List<Story>> GetNewestStoriesAsync(string cacheId = "default") {

            // Append cache ID to the key
            string cacheKey = $"newestStories_{cacheId}";

            // Check if stories are already cached
            if (!_memoryCache.TryGetValue(cacheKey, out List<Story> cachedStories)) {

                // If not cached, create a new HttpClient using the factory
                var client = _httpClientFactory.CreateClient();

                // Fetch the list of story IDs from Hacker News API
                var response = await client.GetStringAsync(_newestStoriesUrl);

                // Deserialize the list of story IDs (JSON array) into a List<int>
                var storyIds = JsonConvert.DeserializeObject<List<int>>(response);

                // For each story ID, fetch its details asynchronously
                var tasks = storyIds.Select(storyId => GetStoryAsync(storyId, client));

                // Wait for all the story detail fetch tasks to complete
                var stories = await Task.WhenAll(tasks);
                cachedStories = stories.ToList();

                // Cache the list of stories with an absolute expiration time of 5 minutes
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                // Set the cached stories in memory cache
                _memoryCache.Set(cacheKey, cachedStories, cacheEntryOptions);
            }

            // Return the cached stories
            return cachedStories;
        }

        // Fetches the details of a single story by its ID
        public async Task<Story> GetStoryAsync(int storyId, HttpClient client) {

            // Make a request to the Hacker News API to get the story details
            var response = await client.GetStringAsync($"{_storyUrl}{storyId}.json?print=pretty");

            // Deserialize the JSON response into a Story object and return it
            return JsonConvert.DeserializeObject<Story>(response);
        }
    }
}