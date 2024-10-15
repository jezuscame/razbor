using API.Features.Extensions;
using AutoMapper;
using Database;
using Database.Tables;
using MediatR;
using Razbor.DAL;
using Responses.UserInformation;

namespace API.Features.Matches
{
    public class GetCurrentPossibleMatchesQuery : IRequest<IList<UserInfoResponse>>
    { }

    // Command Handler
    public class GetCurrentPossibleMatchesQueryHandler : IRequestHandler<GetCurrentPossibleMatchesQuery, IList<UserInfoResponse>>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;


        public GetCurrentPossibleMatchesQueryHandler(
            DatabaseContext databaseContext,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _databaseContext = databaseContext;
            _contextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<IList<UserInfoResponse>> Handle(GetCurrentPossibleMatchesQuery request, CancellationToken cancellationToken)
        {
            var currentUser = _contextAccessor.HttpContext!.GetUserId();

            var destinationMatches = _databaseContext.MatchTable
                .Where(m => m.OriginMatchStatus && m.OriginUser == currentUser)
                .Select(m => m.DestinationUserTable)
                .ToList();

            var originMatches = _databaseContext.MatchTable
                .Where(m => m.DestinationMatchStatus && m.DestinationUser == currentUser)
                .Select(m => m.OriginUserTable)
                .ToList();

            var matches = _databaseContext.UserInfoTable
                .Where(m => m.Username != currentUser)
                .ToList()
                .Except(originMatches)
                .Except(destinationMatches);

            return _mapper.Map<List<UserInfoResponse>>(matches);
        }

    }
}

