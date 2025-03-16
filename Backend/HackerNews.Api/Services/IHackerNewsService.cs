
namespace HackerNews.Api.Services
{
    public interface IHackerNewsService
    {
        Task<List<Story>> GetNewestStoriesAsync(string cacheId = "default");
        Task<Story> GetStoryAsync(int storyId, HttpClient client);
    }
}