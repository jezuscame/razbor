using API.Features.Extensions;
using MediatR;
using Razbor.DAL;
using RequestBodies.Auth;
using Responses.Responses.Auth;
using Services.HashingService;
using Services.TokenService;
using System.Net;

namespace API.Features.Authentication
{
    public class DeleteUserCommand : IRequest<HttpStatusCode>
    {    }

    // Command Handler
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, HttpStatusCode>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHttpContextAccessor _contextAccessor;

        public DeleteUserCommandHandler(DatabaseContext databaseContext, IHttpContextAccessor contextAccessor)
        {
            _databaseContext = databaseContext;
            _contextAccessor = contextAccessor;
        }

        public async Task<HttpStatusCode> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var currentUser = _contextAccessor.HttpContext!.GetUserId();
            var user = _databaseContext.UserInfoTable.Where(u => u.Username == currentUser).FirstOrDefault();
            if (user is null)
                throw new HttpRequestException("User not found", new(), HttpStatusCode.NotFound);
             
            var matches =
                _databaseContext.MatchTable.Where(m => m.DestinationUser == currentUser || m.OriginUser == currentUser)
                    .ToList();

            _databaseContext.RemoveRange(matches);
            _databaseContext.Remove(user);
            await _databaseContext.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

    }
}

