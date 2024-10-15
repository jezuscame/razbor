using API.Features.Authentication;
using API.Features.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestBodies.UserInformation;
using Responses.UserInformation;
using System.Net;
using System.Threading.Tasks;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IMediator mediator, IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPut("update")]
        public async Task<HttpStatusCode> Update([FromBody] UserInfoUpdateRequest data) =>
            await _mediator.Send<HttpStatusCode>(new UserUpdateInfoHandler { Data = data });

        [HttpGet]
        public async Task<UserInfoResponse> GetUserInfo([FromQuery] string user) =>
            await _mediator.Send<UserInfoResponse>(new GetUserInfoHandler { Username = user });

        [HttpGet("profile")]
        public async Task<UserInfoResponse> GetUserInfo() =>
            await _mediator.Send<UserInfoResponse>(new GetUserInfoHandler { Username = _httpContextAccessor.HttpContext.GetUserId() });

    }
}
