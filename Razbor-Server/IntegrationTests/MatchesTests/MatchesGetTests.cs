using FluentAssertions;
using IntegrationTests.Extensions;
using IntegrationTests.Services;
using Responses.UserInformation;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.MatchesTests
{
    public class MatchesGetTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string relativeUri = "/matches";

        public MatchesGetTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async Task RequestAvailableMatches_ContainsAvailableMatch()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();
            var testUser = await _testManager.CreateTestUser();

            // Act
            var response = await _client.SendGetAsync(relativeUri, defaultUser.Token);
            var matches = await response.GetObject<List<UserInfoResponse>>();

            // Assert
            matches.Should().Contain(m => m.Username == testUser.Username);
        }

        [Fact]
        public async Task RequestWithInvalidToken_ReturnsErrorCode()
        {
            // Arrange

            // Act
            var response = await _client.SendGetAsync(relativeUri);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
