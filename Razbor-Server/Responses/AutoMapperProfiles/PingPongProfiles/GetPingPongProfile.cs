using AutoMapper;
using RequestBodies.PingPong;
using Responses.Responses.PingPong;

namespace Responses.AutoMapperProfiles.PingPongProfiles
{
    public class GetPingPongProfile : Profile
    {
        public GetPingPongProfile() => CreateMap<GetPingPong, GetPingPongResponse>();
    }
}
