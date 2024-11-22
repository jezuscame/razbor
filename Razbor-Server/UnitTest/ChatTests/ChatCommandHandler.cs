using AutoMapper;
using API.Features.Chat;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Razbor.DAL;
using Responses.Chat;
using Database.Tables;
using System.Security.Claims;

namespace UnitTest.ChatTests
{
    public class ChatCommandHandlerTests : IDisposable
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private ChatCommandHandler _handler;
        private DatabaseContext _context;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new DatabaseContext(options);

            // Clear any existing data
            _context.MatchTable.RemoveRange(_context.MatchTable);
            _context.ChatTable.RemoveRange(_context.ChatTable);
            _context.SaveChanges();

            // Seed data
            _context.MatchTable.Add(new MatchTable
            {
                Id = "testMatchId",
                OriginUser = "testUserId",
                DestinationUser = "otherUser"
            });
            _context.ChatTable.Add(new ChatTable
            {
                Id = 1,
                MatchId = "testMatchId",
                Sender = "testUserId",
                Message = "Hello",
                Date = DateTime.UtcNow
            });
            _context.SaveChanges();

            // Mock IMapper
            _mapperMock = new Mock<IMapper>();

            // Mock IHttpContextAccessor
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "testUserId")
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Initialize the ChatCommandHandler
            _handler = new ChatCommandHandler(
                _mapperMock.Object,
                _context,
                _httpContextAccessorMock.Object
            );
        }

        [TearDown]
        public void TearDown()
        {
            // Dispose of the database context
            _context?.Dispose();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsMessages()
        {
            // Arrange
            var request = new ChatHandler { MatchData = "testMatchId" };
            var chatData = new[]
            {
                new ChatTable { Id = 1, MatchId = "testMatchId", Sender = "testUserId", Message = "Hello", Date = DateTime.UtcNow }
            };
            var mappedMessages = new[]
            {
                new OneMessage { Sender = "testUserId", Message = "Hello", Date = DateTime.UtcNow }
            }.ToList();

            _mapperMock.Setup(m => m.Map<List<OneMessage>>(It.IsAny<ChatTable[]>())).Returns(mappedMessages);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result.First().Message, Is.EqualTo("Hello"));
            Assert.That(result.First().Sender, Is.EqualTo("testUserId"));
        }

        /*[Test]
        public void Handle_InvalidUser_ThrowsException()
        {
            // Arrange
            var request = new ChatHandler { MatchData = "testMatchId" };
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "invalidUserId")
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act & Assert
            Assert.ThrowsAsync<BadHttpRequestException>(async () => await _handler.Handle(request, CancellationToken.None));
        }*/

        [Test]
        public void Handle_InvalidMatch_ThrowsException()
        {
            // Arrange
            var request = new ChatHandler { MatchData = "nonExistentMatchId" };

            // Act & Assert
            Assert.ThrowsAsync<InvalidOperationException>(async () => await _handler.Handle(request, CancellationToken.None));
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

    }
}
