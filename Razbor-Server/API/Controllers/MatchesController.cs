using API.Features.Matches;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Responses.Matches;
using Responses.Responses.PingPong;
using Responses.UserInformation;
using System.Net;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("matches")]
    public class MatchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IList<UserInfoResponse>> GetMatches() =>
            await _mediator.Send(new GetCurrentPossibleMatchesQuery());

        [HttpPost("match")]
        public async Task<HttpStatusCode> CreateMatch(string user) =>
            await _mediator.Send(new CreateMatchCommand { User = user });

        [HttpGet("matched")]
        public async Task<List<MatchedResponse>> GetMatched() =>
            await _mediator.Send(new GetCurrentMatchesQuery());

    }
}