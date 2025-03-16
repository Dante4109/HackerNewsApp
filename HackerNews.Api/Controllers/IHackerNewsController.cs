using HackerNews.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Api.Controllers
{
    public interface IHackerNewsController
    {
        Task<ActionResult<List<Story>>> GetNewestStoriesAsync();
    }
}