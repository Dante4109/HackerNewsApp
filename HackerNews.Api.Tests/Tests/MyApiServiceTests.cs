using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HackerNews.Api.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace HackerNews.Api.Tests.Tests;
    public class MyApiServiceTests
    {
        [Fact]
        public async Task GetDataAsync_ReturnsExpectedData() {
            // Arrange
            var expectedResponse = "Mocked API Response";

            var mockHandler = new Mock<HttpMessageHandler>();

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

            var httpClient = new HttpClient(mockHandler.Object) {
                BaseAddress = new Uri("https://catfact.ninja/fact")
            };

            var service = new MyApiService(httpClient);

            // Act
            var result = await service.GetDataAsync();

            // Assert
            Assert.Equal(expectedResponse, result);
        }
    }
