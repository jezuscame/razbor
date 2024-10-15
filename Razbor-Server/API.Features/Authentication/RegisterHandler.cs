using AutoMapper;
using MediatR;
using Razbor.DAL;
using RequestBodies.Authentication;
using Responses.Responses.Authentication;
using Database;
using Services.HashingService;
using Services.TokenService;

namespace API.Features.Authentication
{
    public class RegisterHandler : IRequest<RegisterResponse>
    {
        public RegisterRequest Data { get; init; } = null!;
    }

    // Command Handler
    public class RegisterCommandHandler : IRequestHandler<RegisterHandler, RegisterResponse>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;
        private readonly ITokenService _tokenService;

        public RegisterCommandHandler(
            IMapper mapper,
            DatabaseContext databaseContext,
            IHashingService hashingService,
            ITokenService tokenService)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
            _hashingService = hashingService;
            _tokenService = tokenService;
        }

        public async Task<RegisterResponse> Handle(RegisterHandler request, CancellationToken cancellationToken)
        {
            var sameUser = _databaseContext.UserInfoTable.Where(user => user.Username == request.Data.Username).FirstOrDefault();
            if (sameUser is not null)
                throw new HttpRequestException("User already exists", new(), System.Net.HttpStatusCode.Conflict);

            var userInfoTable = _mapper.Map<UserInfoTable>(request.Data);
            userInfoTable.Password = _hashingService.Hash(userInfoTable.Password);
            await _databaseContext.AddTable(userInfoTable);

            return new RegisterResponse { Token = _tokenService.Generate(userInfoTable) };
        }
    }
}

