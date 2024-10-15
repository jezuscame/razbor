using FluentAssertions;
using IntegrationTests.Extensions;
using IntegrationTests.Services;
using Responses.UserInformation;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.MatchesTests
{
    public class MatchPostTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string relativeUri = "/matches/match";

        public MatchPostTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async Task MatchUser_MatchesDoesNotContainUser()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();
            var testUser = await _testManager.CreateTestUser();

            // Act
            var matchResponse = await _client.SendPostAsync(relativeUri, testUser.Token, ("user", defaultUser.Username));
            var matchesResponse = await _client.SendGetAsync("/matches", testUser.Token);
            var matches = await matchesResponse.GetObject<List<UserInfoResponse>>();

            // Assert
            matchResponse.EnsureSuccessStatusCode();
            matchesResponse.EnsureSuccessStatusCode();
            matches.Should().NotContain(m => m.Username == defaultUser.Username);
        }

        [Fact]
        public async Task RequestNonExistantUser_ReturnsErrorCode()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();

            // Act
            var response = await _client.SendPostAsync(relativeUri, String.Empty, defaultUser.Token, ("name", Guid.NewGuid().ToString()));

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
