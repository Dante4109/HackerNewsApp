using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace HackerNewsAPI.Services
{
    public class HackerNewsService
    {
        private readonly HttpClient _client;

        public HackerNewsService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<Story>> GetNewestStoriesAsync()
        {
        
        var response = await _httpClient.GetStringAsync("https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty");
        var storyIds = JsonConvert.DeserializeObject<List<int>>(response);

        var tasks = new List<Task<Story>>();
        foreach (var storyId in storyIds.Take(10)) // Limit to 10 stories for now
        {
            tasks.Add(GetStoryAsync(storyId));
        }

        var stories = await Task.WhenAll(tasks);
        return stories.ToList();
      }
    }

    public class Story
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}