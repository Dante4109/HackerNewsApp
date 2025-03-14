using HackerNews.Api.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HackerNews.Api.Tests.Tests;

public class HackerNewsServiceTests
{
    //private readonly HttpClient _httpClient;
    //private readonly HackerNewsService _hackerNewsService;
    private readonly IMemoryCache _cache;

    //public HackerNewsServiceTests() {
    //    HttpClient = new HttpClient _httpClient;
    //    _hackerNewsService = new HackerNewsService(_mockHttpClient.Object, _cache);
    //}


    [Fact]
    public async Task GetNewStoriesAsync_ReturnsStories() {
        // Arrange
        var expectedResponse = "[1, 2, 3]";

        var mockHandler = new Mock<HttpMessageHandler>();

        //var mockResponse = new HttpResponseMessage(HttpStatusCode.OK) {
        //    Content = new StringContent("[1, 2, 3]")
        //};

        mockHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(expectedResponse)
                });

        var httpClient = new HttpClient(mockHandler.Object);

        //var mockHandler = new MockHttpMessageHandler(mockResponse);
        //_httpClient = new HttpClient(mockHandler);
        
        var service = new HackerNewsService(httpClient, new MemoryCache(new MemoryCacheOptions()));

        // Act
        var result = await service.GetNewestStoriesAsync();

        // Assert
        Assert.NotNull(result);
        // Assert.Equal(3, result.Count);
    }

    //[Fact]
    //public async Task GetStoryAsync_ReturnsStory() {
    //    // Arrange
    //    var fakeStory = new JObject
    //    {
    //        { "id", 1 },
    //        { "title", "Fake Story" },
    //        { "url", "https://example.com" }
    //    };

    //    _mockHttpClient
    //        .Setup(client => client.GetStringAsync(It.IsAny<string>()))
    //        .ReturnsAsync(fakeStory.ToString());

    //    // Act
    //    var result = await _hackerNewsService.GetStoryAsync(1);

    //    // Assert
    //    Assert.NotNull(result);
    //    Assert.Equal("Fake Story", result.Title.ToString());
    //    Assert.Equal("https://example.com", result.Url.ToString());
    //}
}
