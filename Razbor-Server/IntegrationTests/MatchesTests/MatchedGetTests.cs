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
    public class MatchedGetTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string relativeUri = "/matches/matched";

        public const string matchUri = "/matches/match";

        public MatchedGetTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async Task MatchedUser_ReturnsMatchedObject()
        {
            // Arrange
            var testUserOrigin = await _testManager.CreateTestUser();
            var testUserDestination = await _testManager.CreateTestUser();

            await _client.SendPostAsync(matchUri, testUserOrigin.Token, ("user", testUserDestination.Username));
            await _client.SendPostAsync(matchUri, testUserDestination.Token, ("user", testUserOrigin.Username));

            // Act
            var matchedOriginResponse = await _client.SendGetAsync(relativeUri, testUserOrigin.Token);
            var matchedDestinationResponse = await _client.SendGetAsync(relativeUri, testUserDestination.Token);

            var matchedOriginObject = await matchedOriginResponse.GetObject<List<UserInfoResponse>>();
            var matchedDestinationObject = await matchedDestinationResponse.GetObject<List<UserInfoResponse>>();

            // Assert
            matchedOriginResponse.EnsureSuccessStatusCode();
            matchedDestinationResponse.EnsureSuccessStatusCode();

            matchedOriginObject
                 .Should().Contain(m => m.Username == testUserDestination.Username);

            matchedDestinationObject
                .Should().Contain(m => m.Username == testUserOrigin.Username);
        }

        [Fact]
        public async Task NoMatchesInitiated_ReturnsEmptyList()
        {
            // Arrange
            var testUser = await _testManager.CreateTestUser();

            // Act
            var response = await _client.SendGetAsync(relativeUri, testUser.Token);

            // Assert
            (await response
                .GetObject<List<UserInfoResponse>>())
                .Should().BeEmpty();
        }

        [Fact]
        public async Task UseInvalidToken_ReturnsErrorCode()
        {
            // Arrange

            // Act
            var response = await _client.SendGetAsync(relativeUri);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
