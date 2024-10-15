using AutoMapper;
using MediatR;
using Razbor.DAL;
using API.Features.Extensions;
using Microsoft.EntityFrameworkCore;
using Responses.Chat;

namespace API.Features.Chat
{
    public class ChatHandler : IRequest<List<OneMessage>>
    {
        public string MatchData { get; init; } = null!;
    }

    // Command Handler
    public class ChatCommandHandler : IRequestHandler<ChatHandler, List<OneMessage>>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatCommandHandler(
            IMapper mapper,
            DatabaseContext databaseContext,
            IHttpContextAccessor httpContextAccessor)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<OneMessage>> Handle(ChatHandler request, CancellationToken cancellationToken)
        {

            var matchOriginAndDest = await _databaseContext.MatchTable.Where(m => m.Id == request.MatchData).Select(m => new { m.OriginUser, m.DestinationUser}).FirstOrDefaultAsync();

            var userId = _httpContextAccessor.HttpContext.GetUserId();

            if (!(matchOriginAndDest.OriginUser == userId || matchOriginAndDest.DestinationUser == userId)) throw new BadHttpRequestException("The chat is unavailable for the user who requested it");

            var chat = await _databaseContext.ChatTable.Where(c => c.MatchId == request.MatchData).OrderBy(c => c.Date).ToArrayAsync();

            List<OneMessage> result = _mapper.Map<List<OneMessage>>(chat);

            return result;
        }
    }
}

