using HackerNews.Api.Controllers;
using HackerNews.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;

namespace HackerNews.Api.Tests
{
    public class HackerNewsControllerTests
    {
        private readonly HackerNewsController _controller;
        private readonly HttpClient _httpClient;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<IHackerNewsService> _hackerNewsServiceMock;
        private readonly Mock<ILogger<HackerNewsController>> _loggerMock;

        public HackerNewsControllerTests() {
            // Create a mock logger
            _loggerMock = new Mock<ILogger<HackerNewsController>>();

            // Create a mock HttpClient using Moq.Contrib.HttpClient
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            // Setup mock response for the /newstories.json endpoint
            mockHttpMessageHandler
                .SetupRequest(HttpMethod.Get, "https://hacker-news.firebaseio.com/v0/newstories.json?print=pretty")
                .ReturnsResponse("[1, 2, 3]"); // Returning a mock list of story IDs

            // Create HttpClient using the mock handler
            _httpClient = mockHttpMessageHandler.CreateClient();

            //// Initialize HackerNewsServiceMock
            _hackerNewsServiceMock = new Mock<IHackerNewsService>();

            // Initialize the controller with the mock service and logger
            _controller = new HackerNewsController(_hackerNewsServiceMock.Object, _loggerMock.Object);
        }

        // Test: Verify that the controller returns 200 OK with the correct stories
        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsOk_WithStories() {
            // Arrange
            var mockStories = new List<Story>
            {
                    new Story { Id = 1, Title = "Story 1", Url = "www.story1.com" },
                    new Story { Id = 2, Title = "Story 2", Url = "www.story2.com" },
                    new Story { Id = 3, Title = "Story 3", Url = "www.story3.com" }
                };

            _hackerNewsServiceMock.Setup(service => service.GetNewestStoriesAsync("default")).ReturnsAsync(mockStories);

            // Act
            var result = await _controller.GetNewestStoriesAsync();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedStories = Assert.IsType<List<Story>>(okResult.Value);
            Assert.Equal(mockStories.Count, returnedStories.Count);
        }

        // Test: Verify that the controller returns 404 Not Found if no stories are found
        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsNotFound_WhenNoStoriesFound() {
            // Arrange
            _hackerNewsServiceMock.Setup(service => service.GetNewestStoriesAsync("default")).ReturnsAsync(new List<Story>());

            // Act
            var result = await _controller.GetNewestStoriesAsync();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No newest stories found.", notFoundResult.Value);
        }

        // Test: Verify that the controller returns 500 Internal Server Error if an exception occurs
        [Fact]
        public async Task GetNewestStoriesAsync_ReturnsInternalServerError_WhenExceptionOccurs() {
            // Arrange
            _hackerNewsServiceMock.Setup(service => service.GetNewestStoriesAsync("default")).ThrowsAsync(new System.Exception("Internal error"));

            // Act
            var result = await _controller.GetNewestStoriesAsync();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error. Please try again later.", statusCodeResult.Value);
        }
    }
}
