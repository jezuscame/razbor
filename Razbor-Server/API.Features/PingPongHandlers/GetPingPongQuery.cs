using MediatR;
using Responses.Responses.PingPong;
using RequestBodies.PingPong;
using AutoMapper;

namespace API.Features.PingPong
{
    public class GetPingPongQuery : IRequest<GetPingPongResponse>
    {
        public GetPingPong Message { get; init; } = null!;
    }

    // Query Handler
    public class GetPingPongHandler : IRequestHandler<GetPingPongQuery, GetPingPongResponse>
    {
        private readonly IMapper _mapper;

        public GetPingPongHandler(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Task<GetPingPongResponse> Handle(GetPingPongQuery request, CancellationToken cancellationToken) =>
            Task.FromResult(_mapper.Map<GetPingPongResponse>(request.Message));
    
    }
}
