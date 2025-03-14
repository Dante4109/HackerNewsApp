using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Api.Tests
{
    public class MockHttpMessageHandler : HttpMessageHandler {

        private readonly HttpResponseMessage _responseMessage;

        public MockHttpMessageHandler(HttpResponseMessage responseMessage) {
            _responseMessage = responseMessage;
        }
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            return await Task.FromResult(_responseMessage);
        }
    }
}
