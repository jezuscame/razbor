using API.Features.Extensions;
using AutoMapper;
using Database;
using Database.Tables;
using MediatR;
using Razbor.DAL;
using Responses.Matches;
using Responses.UserInformation;

namespace API.Features.Matches
{
    public class GetCurrentMatchesQuery : IRequest<List<MatchedResponse>>
    { }

    // Command Handler
    public class GetCurrentMatchQueryHandler : IRequestHandler<GetCurrentMatchesQuery, List<MatchedResponse>>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;


        public GetCurrentMatchQueryHandler(
            DatabaseContext databaseContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _databaseContext = databaseContext;
            _contextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<List<MatchedResponse>> Handle(GetCurrentMatchesQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _contextAccessor.HttpContext!.GetUserId();

            var destinationMatches = _databaseContext.MatchTable
                .Where(m => m.DestinationMatchStatus && m.OriginMatchStatus && m.OriginUser == currentUser)
                .Select(m => new { User = m.DestinationUserTable, MatchId = m.Id })
                .ToList();

            var originMatches = _databaseContext.MatchTable
                .Where(m => m.DestinationMatchStatus && m.OriginMatchStatus && m.DestinationUser == currentUser)
                .Select(m => new { User = m.OriginUserTable, MatchId = m.Id})
                .ToList();

            var matches = new List<MatchedResponse>();

            foreach (var m in destinationMatches)
            {
                var match = _mapper.Map<MatchedResponse>(m.User);
                match.MatchId = m.MatchId;
                matches.Add(match);
            }

            foreach (var m in originMatches)
            {
                var match = _mapper.Map<MatchedResponse>(m.User);
                match.MatchId = m.MatchId;
                matches.Add(match);
            }

            return matches;
        }

    }
}

