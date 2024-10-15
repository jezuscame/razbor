using AutoMapper;
using MediatR;
using Razbor.DAL;
using Responses.UserInformation;

namespace API.Features.Authentication
{
    public class GetUserInfoHandler : IRequest<UserInfoResponse>
    {
        public string Username { get; init; }
    }

    // Command Handler
    public class GetInfoHandler : IRequestHandler<GetUserInfoHandler, UserInfoResponse>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;

        public GetInfoHandler(
            IMapper mapper,
            DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
        }

        public async Task<UserInfoResponse> Handle(GetUserInfoHandler request, CancellationToken cancellationToken)
        {
            string userId = request.Username;
            UserInfoTable? userToGet = _databaseContext.UserInfoTable.FirstOrDefault(user => user.Username == userId);

            if (userToGet == null)
            {
                throw new HttpRequestException("User not found", new(), System.Net.HttpStatusCode.NotFound);
            }

            return _mapper.Map<UserInfoResponse>(userToGet);
        }
    }
}

