using MediatR;
using Razbor.DAL;
using RequestBodies.Auth;
using Responses.Responses.Auth;
using Services.HashingService;
using Services.TokenService;

namespace API.Features.Authentication
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public LoginRequest Data { get; init; } = null!;
    }

    // Command Handler
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IHashingService _hashingService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(IHashingService hashingService, DatabaseContext databaseContext, ITokenService tokenService)
        {
            _databaseContext = databaseContext;
            _hashingService = hashingService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            if (request.Data.Username is null || request.Data.Password is null)
                throw new BadHttpRequestException("Bad Request");

            var user = _databaseContext.UserInfoTable
                .FirstOrDefault(u => u.Username == request.Data.Username);
            if (user is null || !_hashingService.Verify(user.Password, request.Data.Password))
                throw new BadHttpRequestException("Bad credentials");


            else return new LoginResponse { Token = _tokenService.Generate(user) };
        }

    }
}

