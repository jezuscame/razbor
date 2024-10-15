using API.Features.Extensions;
using Database;
using Database.Tables;
using MediatR;
using Razbor.DAL;
using System.Net;

namespace API.Features.Matches
{
    public class CreateMatchCommand : IRequest<HttpStatusCode>
    {
        public string User { get; set; } = null!;
    }

    // Command Handler
    public class CreateMatchCommandHandler : IRequestHandler<CreateMatchCommand, HttpStatusCode>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public CreateMatchCommandHandler(DatabaseContext databaseContext, IHttpContextAccessor httpContextAccessor)
        {
            _databaseContext = databaseContext;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<HttpStatusCode> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _contextAccessor.HttpContext!.GetUserId();
            if (request.User == currentUser)
                throw new BadHttpRequestException("User can not match himself");

            if (_databaseContext.UserInfoTable.Where(u => u.Username == request.User).FirstOrDefault() is null)
                throw new BadHttpRequestException("Requested user not found");

            var matchObject = _databaseContext.MatchTable
                .Where(m => 
                    (m.OriginUser == currentUser || m.DestinationUser == currentUser) &&
                    (m.OriginUser == request.User || m.DestinationUser == request.User))
                .FirstOrDefault();
            
            if (matchObject is not null)
            {
                if (matchObject.OriginUser == request.User)
                    matchObject.DestinationMatchStatus = true;

                if (matchObject.DestinationUser == request.User)
                    matchObject.OriginMatchStatus = true;


                await _databaseContext.SaveChangesAsync();
                return HttpStatusCode.OK;
            }

            await _databaseContext.AddTable(new MatchTable
            {
                Id = Guid.NewGuid().ToString(),
                OriginUser = currentUser,
                OriginMatchStatus = true,
                DestinationUser = request.User,
                DestinationMatchStatus = false,
            });
            return HttpStatusCode.OK;
        }

    }
}

