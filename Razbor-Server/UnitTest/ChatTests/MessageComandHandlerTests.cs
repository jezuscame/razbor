using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using API.Features.Chat;
using Razbor.DAL;
using Database.Tables;
using RequestBodies.Chat;
using API.Features.Extensions;
using System.Security.Claims;

namespace UnitTest.ChatTests
{
    public class MessageCommandHandlerTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private DatabaseContext _context;
        private MessageCommandHandler _handler;

        [SetUp]
        public void Setup()
        {
            // Set up in-memory database
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase_MessageHandler")
                .Options;

            _context = new DatabaseContext(options);

            // Clear any existing data
            _context.UserInfoTable.RemoveRange(_context.UserInfoTable);
            _context.MatchTable.RemoveRange(_context.MatchTable);
            _context.ChatTable.RemoveRange(_context.ChatTable);
            _context.SaveChanges();

            // Seed data
            _context.UserInfoTable.Add(new UserInfoTable
            {
                Username = "recipientUser",
                Password = "password",
                FirstName = "Recipient",
                LastName = "User",
                UserGender = Database.Enums.Gender.Female,
                Birthday = DateTime.UtcNow.AddYears(-25),
                Email = "recipient@example.com",
                Description = "Recipient description",
                SelectedSong = "Song A",
                PrimaryPicture = "http://example.com/image.jpg"
            });
            _context.SaveChanges();
            _context.MatchTable.Add(new MatchTable
            {
                Id = "testMatchId",
                OriginUser = "testUserId2",
                DestinationUser = "recipientUser",
                OriginMatchStatus = true,
                DestinationMatchStatus = true
            });
            _context.SaveChanges();
            var matches = _context.MatchTable.ToList();


            // Mock HttpContextAccessor
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
        new Claim("userId", "testUserId")
    }));

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Initialize the MessageCommandHandler
            _handler = new MessageCommandHandler(_context, _httpContextAccessorMock.Object);
        }



        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task Handle_ValidRequest_ReturnsHttpStatusCodeOK()
        {
            // Arrange
            var request = new MessageHandler
            {
                Data = new SendMessageRequest
                {
                    Recipient = "recipientUser",
                    Message = "Hello!"
                }
            };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.That(result, Is.EqualTo(HttpStatusCode.OK));
            var chatMessageExists = await _context.ChatTable.AnyAsync(c =>
                c.Message == "Hello!" && c.Sender == "testUserId2");
            Assert.That(chatMessageExists, Is.True);
        }



        [Test]
        public void Handle_InvalidRecipient_ThrowsException()
        {
            // Arrange
            var request = new MessageHandler
            {
                Data = new SendMessageRequest
                {
                    Recipient = "nonExistentUser",
                    Message = "Hello!"
                }
            };

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () =>
                await _handler.Handle(request, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("There is no such recipient nonExistentUser"));
        }

        [Test]
        public void Handle_NoMatch_ThrowsException()
        {
            // Arrange
            var request = new MessageHandler
            {
                Data = new SendMessageRequest
                {
                    Recipient = "recipientUser",
                    Message = "Hello!"
                }
            };

            // Remove the match to simulate "no match" condition
            var match = _context.MatchTable.FirstOrDefault(m => m.Id == "testMatchId");
            if (match != null)
            {
                _context.MatchTable.Remove(match);
                _context.SaveChanges();
            }

            // Act & Assert
            var ex = Assert.ThrowsAsync<BadHttpRequestException>(async () =>
                await _handler.Handle(request, CancellationToken.None));
            Assert.That(ex.Message, Is.EqualTo("It is impossible to send message to recipientUser due to no match"));
        }
    }
}
