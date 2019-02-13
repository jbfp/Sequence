using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Sequence.Api.Test
{
    public sealed class BoardControllerIntegrationTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string BasePath = "/boards";

        private readonly WebApplicationFactory<Startup> _factory;

        public BoardControllerIntegrationTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData(BasePath)]
        [InlineData(BasePath + "/sequence")]
        public async Task RoutesAreProtected(string route)
        {
            using (var client = _factory.CreateDefaultClient())
            using (var response = await client.GetAsync(route))
            {
                Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            }
        }

        [Theory]
        [InlineData("sequence")]
        [InlineData("seQuenCe")]
        [InlineData("OneEyedJACk")]
        [InlineData("oneeyedjack")]
        public async Task BoardTypeFromKey(string key)
        {
            using (var response = await GetAsync(key))
            {
                Assert.True(response.IsSuccessStatusCode);
            }
        }

        [Theory]
        [InlineData("i dont exist")]
        [InlineData("foiwejfowiefjowiejf")]
        [InlineData("390r23r9#####@")]
        public async Task InvalidBoardTypeBadRequest(string key)
        {
            using (var response = await GetAsync(key))
            {
                Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            }
        }

        [Fact]
        public async Task BoardTypeCaching()
        {
            using (var response = await GetAsync("sequence"))
            {
                Assert.NotNull(response.Headers.CacheControl);
                Assert.True(response.Headers.CacheControl.Public);
                Assert.NotNull(response.Headers.CacheControl.MaxAge);
                Assert.Equal(response.Headers.CacheControl.MaxAge.Value, TimeSpan.FromDays(4 * 30.4375));
            }
        }

        private HttpClient CreateAuthorizedClient(string playerId = "test_player")
        {
            if (playerId == null)
            {
                throw new ArgumentNullException(nameof(playerId));
            }

            var client = _factory.CreateDefaultClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("test_player");
            return client;
        }

        private async Task<HttpResponseMessage> GetAsync(string path = null)
        {
            if (path != null)
            {
                path = $"/{Uri.EscapeDataString(path)}";
            }

            string requestUri = $"{BasePath}{path}";

            using (var client = CreateAuthorizedClient())
            {
                return await client.GetAsync(requestUri);
            }
        }
    }
}
