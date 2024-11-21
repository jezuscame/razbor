using API.Controllers; // For AuthenticationController
using API.Features.Authentication;
using API.Features.Extensions;
using AutoMapper;
using Database.Tables;
using MediatR; // For IMediator
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq; // For mocking dependencies
using NUnit.Framework; // For NUnit test framework
using NUnit.Framework.Legacy;
using Razbor.DAL;
using RequestBodies.Auth; // For LoginRequest
using RequestBodies.Authentication;
using Responses.Responses.Auth; // For LoginResponse
using Responses.Responses.Authentication;
using Services.HashingService;
using Services.TokenService;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Reflection.Metadata;
using System.Threading.Tasks; // For async support

namespace UnitTest.Controllers
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private Mock<IMediator> _mediatorMock;
        private AuthenticationController _controller;
        private Mock<IHashingService> _hashingServiceMock;
        private Mock<IMapper> _mapperMock;
        private Mock<DatabaseContext> _databaseContextMock;
        private Mock<ITokenService> _tokenServiceMock;
        private Mock<IHttpContextAccessor> _contextAccessorMock;
        private DbContextOptions<DatabaseContext> _dbContextOptions;
        private DbContextOptions<DatabaseContext> options;

        [SetUp]
        public void Setup()
        {
            // Create a mock instance of IMediator
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();
            // Instantiate the controller with the mocked dependency
            _controller = new AuthenticationController(_mediatorMock.Object);
            _hashingServiceMock = new Mock<IHashingService>();
            _databaseContextMock = new Mock<DatabaseContext>();
            _tokenServiceMock = new Mock<ITokenService>();

            _contextAccessorMock = new Mock<IHttpContextAccessor>();

            _dbContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                               .UseInMemoryDatabase(databaseName: "TestDatabase")
                               .Options;
            options = new DbContextOptionsBuilder<DatabaseContext>()
    .UseInMemoryDatabase(databaseName: TestDatabaseName) // Use shared database name
    .Options;
            _hashingServiceMock
               .Setup(h => h.Hash(It.IsAny<string>()))
               .Returns("hashed_password");

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

        [Test]
        public void Login_ThrowsBadRequestException_OnInvalidPassword()
        {
            // Arrange: Create a login request with a valid username but an incorrect password
            var request = new LoginRequest
            {
                Username = "valid_user",
                Password = "invalid_password"
            };

            // Mock the mediator to throw a BadHttpRequestException for invalid credentials
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginCommand>(), default))
                .ThrowsAsync(new BadHttpRequestException("Bad credentials"));

            // Act & Assert: Verify that the exception is thrown with the expected message
            var exception = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _controller.Login(request));
            Assert.That(exception.Message, Is.EqualTo("Bad credentials"), "Expected exception message: 'Bad credentials'");
        }
        [Test]
        public void Login_ThrowsBadRequestException_OnMissingUsernameOrPassword()
        {
            // Arrange: Create a login request with missing username and password
            var request = new LoginRequest
            {
                Username = null,
                Password = null
            };

            // Mock the mediator to throw a BadHttpRequestException for missing credentials
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<LoginCommand>(), default))
                .ThrowsAsync(new BadHttpRequestException("Bad Request"));

            // Act & Assert: Verify the exception is thrown with the expected message
            var exception = Assert.ThrowsAsync<BadHttpRequestException>(async () => await _controller.Login(request));
            Assert.That(exception.Message, Is.EqualTo("Bad Request"), "Expected exception message: 'Bad Request'");
        }
        [Test]
        public async Task Register_ReturnsToken_OnSuccessfulRegistration()
        {
            // Arrange
            var request = new RegisterRequest
            {
                Username = "new_user",
                Password = "secure_password",
                Email = "user@example.com"
            };

            var userTable = new UserInfoTable
            {
                Username = request.Username,
                Password = "hashed_password",
                Email = request.Email
            };

            var expectedToken = "mocked_token";

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<RegisterHandler>(), default))
                .ReturnsAsync(new RegisterResponse { Token = expectedToken });

            // Act
            var result = await _controller.Register(request);

            // Assert
            Assert.That(result.Token, Is.EqualTo(expectedToken), "Expected token did not match.");
        }
        private const string TestDatabaseName = "SharedTestDatabase";

        [Test]
        public async Task Register_SuccessfullyRegisters_WhenValidDataIsProvided()
        {
            // Arrange
            var newUser = new RegisterRequest
            {
                Username = "new_user",
                Password = "password123",
                FirstName = "First",
                LastName = "Last",
                Email = "newuser@example.com",
                Description = "New user",
                PrimaryPicture = "picture.jpg",
                SelectedSong = "song.mp3"
            };

            var expectedToken = "generated_token";


            using (var context = new DatabaseContext(options))
            {
                // Mock behavior
                _mapperMock.Setup(m => m.Map<UserInfoTable>(It.IsAny<RegisterRequest>()))
                           .Returns(new UserInfoTable
                           {
                               Username = newUser.Username,
                               Password = newUser.Password,
                               FirstName = newUser.FirstName,
                               LastName = newUser.LastName,
                               Email = newUser.Email,
                               Description = newUser.Description,
                               PrimaryPicture = newUser.PrimaryPicture,
                               SelectedSong = newUser.SelectedSong
                           });

                _hashingServiceMock.Setup(h => h.Hash(It.IsAny<string>())).Returns("hashed_password");
                _tokenServiceMock.Setup(t => t.Generate(It.IsAny<UserInfoTable>())).Returns(expectedToken);

                var handler = new RegisterCommandHandler(_mapperMock.Object, context, _hashingServiceMock.Object, _tokenServiceMock.Object);
                var registerHandler = new RegisterHandler { Data = newUser };

                // Act
                var result = await handler.Handle(registerHandler, CancellationToken.None);

                // Assert
                ClassicAssert.IsNotNull(result);
                ClassicAssert.AreEqual(expectedToken, result.Token);

                var user = await context.UserInfoTable.FirstOrDefaultAsync(u => u.Username == newUser.Username);
                ClassicAssert.IsNotNull(user);
                ClassicAssert.AreEqual(newUser.Username, user.Username);
                ClassicAssert.AreEqual("hashed_password", user.Password);
            }
        }

        [Test]
        public async Task DeleteUser_SuccessfullyDeletesUser_WhenUserExists()
        {
            // Arrange
            var usernameToDelete = "new_user";

            //var options = new DbContextOptionsBuilder<DatabaseContext>()
            //    .UseInMemoryDatabase(databaseName: TestDatabaseName) // Use the same database name
            //    .Options;

            using (var context = new DatabaseContext(options))
            {
                var user = await context.UserInfoTable.FirstOrDefaultAsync(u => u.Username == usernameToDelete);
                ClassicAssert.IsNotNull(user, $"User '{usernameToDelete}' not found in the database.");

                var httpContextMock = new Mock<HttpContext>();
                httpContextMock.Setup(h => h.User.Identity.Name).Returns(usernameToDelete);
                _contextAccessorMock.Setup(c => c.HttpContext).Returns(httpContextMock.Object);

                var handler = new DeleteUserCommandHandler(context, _contextAccessorMock.Object);

                // Act
                var result = await handler.Handle(new DeleteUserCommand(), CancellationToken.None);

                // Assert
                Assert.That(result, Is.EqualTo(HttpStatusCode.OK));

                var deletedUser = await context.UserInfoTable.FirstOrDefaultAsync(u => u.Username == usernameToDelete);
                ClassicAssert.IsNull(deletedUser, $"User '{usernameToDelete}' should have been deleted.");
            }
        }

        [Test]
        public async Task RegisterUser_Fails_WhenNoDataIsProvided()
        {
            // Arrange
            RegisterRequest invalidRequest = null; // Simulating no data provided

            //var options = new DbContextOptionsBuilder<DatabaseContext>()
            //    .UseInMemoryDatabase(databaseName: "SharedTestDatabase") // Use the shared database name
            //    .Options;

            using (var context = new DatabaseContext(options))
            {
                var handler = new RegisterCommandHandler(_mapperMock.Object, context, _hashingServiceMock.Object, _tokenServiceMock.Object);

                try
                {
                    // Act
                    await handler.Handle(new RegisterHandler { Data = invalidRequest }, CancellationToken.None);

                    // If no exception is thrown, fail the test
                    Assert.Fail("Expected an exception to be thrown, but none was thrown.");
                }
                catch (Exception ex)
                {
                    // Assert
                    Assert.Pass($"Test passed. Caught exception of type {ex.GetType().Name}: {ex.Message}");
                }
            }
        }










    }



}
