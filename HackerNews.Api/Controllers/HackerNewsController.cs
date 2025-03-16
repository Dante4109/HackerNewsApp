using HackerNews.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    // HackerNewsController is responsible for handling requests related to Hacker News stories
    public class HackerNewsController : ControllerBase, IHackerNewsController
    {
        private readonly IHackerNewsService _hackerNewsService;
        private readonly ILogger<HackerNewsController> _logger;

        // Constructor with hackerNews Service and logger dependency 
        public HackerNewsController(IHackerNewsService hackerNewsService, ILogger<HackerNewsController> logger) {
            _hackerNewsService = hackerNewsService;
            _logger = logger;
        }

        [HttpGet(Name = "newest")]
        public async Task<ActionResult<List<Story>>> GetNewestStoriesAsync() {

            // Try to fetch the newest stories from the Hacker News service
            try {
                var stories = await _hackerNewsService.GetNewestStoriesAsync();

                // If no stories are found, return a 404 status
                if (stories == null || stories.Count == 0) {
                    _logger.LogWarning("No stories found.");
                    return NotFound("No newest stories found.");
                }

                return Ok(stories);

            } catch (Exception ex) {
                // Log the error and return an internal server error
                _logger.LogError(ex, "An error occurred while retrieving newest stories.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
    }
}