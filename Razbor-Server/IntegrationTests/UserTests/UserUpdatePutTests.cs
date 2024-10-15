using FluentAssertions;
using IntegrationTests.Extensions;
using IntegrationTests.Services;
using RequestBodies.UserInformation;
using Responses.UserInformation;
using System;
using System.Net.Http;
using Xunit;

namespace IntegrationTests.UserTests
{
    public class UserUpdatePutTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string relativeUri = "/user/update";

        public UserUpdatePutTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async void RequestUpdateInformation_ChangesInformation()
        {
            // Arrange
            var testUser = await _testManager.CreateTestUser();
            var updateRequest = new UserInfoUpdateRequest
            {
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
            };

            // Act
            var response = await _client.SendPutAsync(relativeUri, updateRequest, testUser.Token);
            var currentProfile = await (await _client.SendGetAsync("/user/profile", testUser.Token)).GetObject<UserInfoResponse>();

            // Assert
            currentProfile.FirstName.Should().Be(updateRequest.FirstName);
            currentProfile.LastName.Should().Be(updateRequest.LastName);
        }

        [Fact]
        public async void RequestUpdateInformationInvalidToken_ErrorCode()
        {
            // Arrange
            var updateRequest = new UserInfoUpdateRequest();

            // Act
            var response = await _client.SendPutAsync(relativeUri, updateRequest);

            // Assert
            response.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
