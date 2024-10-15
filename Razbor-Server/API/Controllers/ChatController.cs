using API.Features.Authentication;
using API.Features.Matches;
using API.Features.Chat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestBodies.Chat;
using Responses.Responses.PingPong;
using Responses.UserInformation;
using System.Net;
using Responses.Chat;
using RequestBodies.PingPong;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("chat")]
    public class ChatController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ChatController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route("message")]
        public async Task<HttpStatusCode> SendMessage([FromBody] SendMessageRequest request) =>
            await _mediator.Send(new MessageHandler { Data = request });

        [HttpGet]
        public async Task<List<OneMessage>> GetChat([FromQuery] string matchId) =>
            await _mediator.Send(new ChatHandler { MatchData = matchId });
    }
}