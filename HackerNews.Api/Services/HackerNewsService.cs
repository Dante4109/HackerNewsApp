using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;

namespace HackerNews.Api.Services
{
    public class HackerNewsService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _client;

        public HackerNewsService(HttpClient client, IMemoryCache cache) {
            
            _client = client;
            _cache = cache;
        }

        public async Task<List<Story>> GetNewestStoriesAsync() {
            if (!_cache.TryGetValue("newestStories", out List<Story> cachedStories)) {
                var response = await _client.GetStringAsync("https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty");
                var storyIds = JsonConvert.DeserializeObject<List<int>>(response);

                //var tasks = new List<Task<Story>>();
                //foreach (var storyId in storyIds.Take(100)) {
                //    tasks.Add(GetStoryAsync(storyId));
                //}

                var tasks = storyIds.Select(storyId => GetStoryAsync(storyId));

                var stories = await Task.WhenAll(tasks);
                cachedStories = stories.ToList();

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set("newestStories", cachedStories, cacheEntryOptions);
            }

            return cachedStories;
        }

        public async Task<Story> GetStoryAsync(int storyId) {
            var response = await _client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");
            return JsonConvert.DeserializeObject<Story>(response);
        }
    }
}