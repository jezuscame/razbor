using API.Features.Authentication;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestBodies.Auth;
using RequestBodies.Authentication;
using Responses.Responses.Auth;
using Responses.Responses.Authentication;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<LoginResponse> Login([FromBody] LoginRequest request) =>
            await _mediator.Send<LoginResponse>(new LoginCommand { Data = request });

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<RegisterResponse> Register([FromBody] RegisterRequest request) =>
            await _mediator.Send<RegisterResponse>(new RegisterHandler { Data = request });

        [HttpDelete("delete")]
        [Authorize]
        public async Task<HttpStatusCode> Delete() =>
            await _mediator.Send<HttpStatusCode>(new DeleteUserCommand());

    }
}
