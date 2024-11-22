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
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrEmpty(request.MatchData)) throw new InvalidOperationException("MatchData is required.");

            var match = await _databaseContext.MatchTable.FirstOrDefaultAsync(m => m.Id == request.MatchData, cancellationToken);
            if (match == null) throw new InvalidOperationException("Match not found.");

            var chatMessages = await _databaseContext.ChatTable
                .Where(c => c.MatchId == match.Id)
                .OrderBy(c => c.Date)
                .ToArrayAsync(cancellationToken);

            return _mapper.Map<List<OneMessage>>(chatMessages);
        }

    }
}

