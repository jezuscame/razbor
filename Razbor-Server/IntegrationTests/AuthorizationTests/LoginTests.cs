using FluentAssertions;
using IntegrationTests.Extensions;
using IntegrationTests.Services;
using RequestBodies.Auth;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.AuthorizationTests
{
    public class LoginTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string loginRelativeUri = "/login";

        public LoginTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async Task LoginWithCorrentData_ReturnsToken()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();
            var requestBody = new LoginRequest { Username = defaultUser.Username, Password = defaultUser.Password };

            // Act
            var response = await _client.SendPostAsync(loginRelativeUri, requestBody);

            // Assert
            response.EnsureSuccessStatusCode();
        }


        [Fact]
        public async Task LoginWithWrongPassword_ReturnsErrorCode()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();
            var requestBody = new LoginRequest 
            {
                Username = defaultUser.Username,
                Password = Guid.NewGuid().ToString()
            };

            // Act
            var registerRequest = await _client.SendPostAsync(loginRelativeUri, requestBody);

            // Assert
            registerRequest.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task LoginWithUnknownCredentials_ReturnsErrorCode()
        {
            // Arrange
            var requestBody = new LoginRequest
            {
                Username = Guid.NewGuid().ToString(),
                Password = Guid.NewGuid().ToString()
            };

            // Act
            var registerRequest = await _client.SendPostAsync(loginRelativeUri, requestBody);

            // Assert
            registerRequest.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
