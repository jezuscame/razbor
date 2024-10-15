using API.Features.PingPong;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestBodies.PingPong;
using Responses.Responses.PingPong;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("ping")]
    public class PingPongController : ControllerBase
    {
        private readonly ILogger<PingPongController> _logger;
        private readonly IMediator _mediator;

        public PingPongController(IMediator mediator, ILogger<PingPongController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<GetPingPongResponse> GetPingPong([FromQuery] GetPingPong request)
        {
            _logger.LogInformation("Received a ping request with message: {Message}", request.Message);

            var response = await _mediator.Send<GetPingPongResponse>(new GetPingPongQuery { Message = request });

            _logger.LogInformation("Returning pong response with message: {Message}", request.Message);

            return response;
        }

    }
}