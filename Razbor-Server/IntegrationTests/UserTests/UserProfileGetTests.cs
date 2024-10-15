using FluentAssertions;
using IntegrationTests.Extensions;
using IntegrationTests.Services;
using Responses.UserInformation;
using System.Net.Http;
using Xunit;

namespace IntegrationTests.UserTests
{
    public class UserProfileGetTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string userRelativeUri = "/user/profile";

        public UserProfileGetTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async void RequestsInformation_ReturnsProvidedInformation()
        {
            // Arrange
            var testUserObject = _testManager.GenerateRegisterRequest();
            var testUser = await _testManager.CreateTestUser(testUserObject);

            // Act
            var response = await _client.SendGetAsync(userRelativeUri, testUser.Token);
            var responseObject = await response.GetObject<UserInfoResponse>();

            // Assert
            responseObject.FirstName.Should().Be(testUserObject.FirstName);
            responseObject.LastName.Should().Be(testUserObject.LastName);

        }

        [Fact]
        public async void RequestsInformationInvalidToken_ReturnsErrorStatusCode()
        {
            // Arrange

            // Act
            var response = await _client.SendGetAsync(userRelativeUri);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();

        }
    }
}
