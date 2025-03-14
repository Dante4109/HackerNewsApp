using HackerNews.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly HackerNewsService _hackerNewsService;

        public HackerNewsController(HackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet(Name = "newest")]
        public async Task<ActionResult<List<Story>>> GetNewestStories()
        {
            var stories = await _hackerNewsService.GetNewestStoriesAsync();
            return Ok(stories);
        }
    }
}