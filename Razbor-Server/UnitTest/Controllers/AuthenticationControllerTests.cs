using API.Controllers; // For AuthenticationController
using API.Features.Authentication;
using MediatR; // For IMediator
using Moq; // For mocking dependencies
using NUnit.Framework; // For NUnit test framework
using NUnit.Framework.Legacy;
using RequestBodies.Auth; // For LoginRequest
using Responses.Responses.Auth; // For LoginResponse
using System.Threading.Tasks; // For async support

namespace UnitTest.Controllers
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private AuthenticationController _controller;

        [SetUp]
        public void Setup()
        {
            // Create a mock instance of IMediator
            _mediatorMock = new Mock<IMediator>();

            // Instantiate the controller with the mocked dependency
            _controller = new AuthenticationController(_mediatorMock.Object);
        }

        [Test]
        public async Task Login_ReturnsExpectedToken_OnValidRequest()
        {
            // Arrange
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "password123"
            };

            var expectedResponse = new LoginResponse
            {
                Token = "mock-token"
            };

            // Mock the mediator's Send method to return the expected response
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginCommand>(), default)) // Assuming LoginCommand is used here
                .ReturnsAsync(expectedResponse);

            // Act
            var result = await _controller.Login(request);

            // Assert
            ClassicAssert.IsNotNull(result); // Ensure the result is not null
            ClassicAssert.AreEqual(expectedResponse.Token, result.Token); // Ensure the token matches
        }
    }
}
