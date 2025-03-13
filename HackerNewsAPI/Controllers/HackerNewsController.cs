namespace HackerNewsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HackerNewsController : ControllerBase
    {
        private readonly HackerNewsService _hackerNewsService;

        public HackerNewsController(HackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        [HttpGet("newest")]
        public async Task<ActionResult<List<Story>>> GetNewestStories()
        {
            var stories = await _hackerNewsService.GetNewestStoriesAsync();
            return Ok(stories);
        }
    }
}