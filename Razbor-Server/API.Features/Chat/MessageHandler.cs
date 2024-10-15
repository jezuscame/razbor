using MediatR;
using Razbor.DAL;
using Database;
using System.Net;
using RequestBodies.Chat;
using API.Features.Extensions;
using Database.Tables;
using Microsoft.EntityFrameworkCore;

namespace API.Features.Chat
{
    public class MessageHandler : IRequest<HttpStatusCode>
    {
        public SendMessageRequest Data { get; init; } = null!;
    }

    // Command Handler
    public class MessageCommandHandler : IRequestHandler<MessageHandler, HttpStatusCode>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MessageCommandHandler(
            DatabaseContext databaseContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _databaseContext = databaseContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HttpStatusCode> Handle(MessageHandler request, CancellationToken cancellationToken)
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();

            var recipient = _databaseContext.UserInfoTable
                .FirstOrDefault(u => u.Username == request.Data.Recipient);

            if (recipient == null) throw new BadHttpRequestException($"There is no such recipient {request.Data.Recipient}");

            var matchId = await _databaseContext.MatchTable
                .Where(m =>
                    ((m.OriginUser == userId && m.DestinationUser == request.Data.Recipient) ||
                    (m.OriginUser == request.Data.Recipient && m.DestinationUser == userId)) && (m.DestinationMatchStatus && m.OriginMatchStatus)
                )
                .Select(match => match.Id)
                .FirstOrDefaultAsync();

            if (matchId == null) throw new BadHttpRequestException($"It is impossible to send message to {request.Data.Recipient} due to no match");


            await _databaseContext.AddTable(new ChatTable
            {
                Message = request.Data.Message,
                Date = DateTime.UtcNow,
                MatchId = matchId.ToString(),
                Sender = userId
            });

            return HttpStatusCode.OK;
        }
    }
}

