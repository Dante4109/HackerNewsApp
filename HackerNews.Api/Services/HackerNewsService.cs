using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;

namespace HackerNews.Api.Services
{
    public class HackerNewsService
    {
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _httpClientFactory;

        public HackerNewsService(IHttpClientFactory httpClientFactory, IMemoryCache cache) {
            _httpClientFactory = httpClientFactory;
            _cache = cache;
        }

        public async Task<List<Story>> GetNewestStoriesAsync() {
            if (!_cache.TryGetValue("newestStories", out List<Story> cachedStories)) {
                var client = _httpClientFactory.CreateClient();

                var response = await client.GetStringAsync("https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty");
                var storyIds = JsonConvert.DeserializeObject<List<int>>(response);

                var tasks = storyIds.Select(storyId => GetStoryAsync(storyId, client));

                var stories = await Task.WhenAll(tasks);
                cachedStories = stories.ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set("newestStories", cachedStories, cacheEntryOptions);
            }

            return cachedStories;
        }

        public async Task<Story> GetStoryAsync(int storyId, HttpClient client) {
            var response = await client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");
            return JsonConvert.DeserializeObject<Story>(response);
        }
    }
}