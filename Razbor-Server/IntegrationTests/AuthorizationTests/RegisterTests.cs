using FluentAssertions;
using IntegrationTests.Extensions;
using IntegrationTests.Services;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.AuthorizationTests
{
    public class RegisterTests
    {
        public readonly TestManager _testManager;

        public readonly HttpClient _client;

        public const string registerRelativeUri = "/register";


        public RegisterTests(TestManager testManager, HttpClient client)
        {
            _testManager = testManager;
            _client = client;
        }

        [Fact]
        public async Task RegisterNewUser_ReturnsToken()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();
            
            // Act

            // Assert
            defaultUser.Token.Should().NotBeEmpty();
        }


        [Fact]
        public async Task RegisterUserWithExistingUsername_ReturnsErrorCode()
        {
            // Arrange
            var defaultUser = await _testManager.GetDefaultTestUser();
            var newUser = _testManager.GenerateRegisterRequest(defaultUser.Username, defaultUser.Password);

            // Act
            var registerRequest = await _client.SendPostAsync(registerRelativeUri, newUser);

            // Assert
            registerRequest.IsSuccessStatusCode.Should().BeFalse();
        }
    }
}
