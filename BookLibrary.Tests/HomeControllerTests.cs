using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BookLibrary.Tests
{
    [Collection("TestCollection")]
    public class HomeControllerTests
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public HomeControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task IndexOnGet_WithNoInput_ReturnsWelcomePage()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.NotNull(response.Content);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Welcome", responseString);
        }
    }
}
