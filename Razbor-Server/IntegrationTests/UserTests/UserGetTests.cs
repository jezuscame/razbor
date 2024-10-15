using FluentAssertions;
using IntegrationTests.Extensions;
using IntegrationTests.Services;
using Responses.UserInformation;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.UserTests
{
    public class UserGetTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string userRelativeUri = "/user";

        public UserGetTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async Task RequestUserData_ReturnsValidData()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();
            var testUser = await _testManager.CreateTestUser();
            
            // Act
            var response = await _client.SendGetAsync(userRelativeUri, defaultUser.Token, ("user", testUser.Username));
            var responseObject = await response.GetObject<UserInfoResponse>();

            // Assert
            responseObject.Username.Should().Be(testUser.Username);
        }

        [Fact]
        public async Task RequestNonExistantUser_ReturnsErrorCode()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();

            // Act
            var response = await _client.SendGetAsync(userRelativeUri, defaultUser.Token, ("user", Guid.NewGuid().ToString()));

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
