using Newtonsoft.Json;

namespace HackerNews.Api.Services
{
    public class HackerNewsService
    {
        private readonly HttpClient _client;

        public HackerNewsService(HttpClient client) {
            _client = client;
        }

        public async Task<List<Story>> GetNewestStoriesAsync() {

            var response = await _client.GetStringAsync("https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty");
            var storyIds = JsonConvert.DeserializeObject<List<int>>(response);

            var tasks = storyIds.Take(10).Select(storyId => GetStoryAsync(storyId)// Limit to 10 stories for now
            ).ToList();
            var stories = await Task.WhenAll(tasks);
            return stories.ToList();
        }

        public async Task<Story> GetStoryAsync(int storyId) {
            var response = await _client.GetStringAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");
            return JsonConvert.DeserializeObject<Story>(response);
        }
    }
}