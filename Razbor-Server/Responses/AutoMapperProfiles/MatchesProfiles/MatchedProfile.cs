using AutoMapper;
using Responses.Matches;

namespace Responses.AutoMapperProfiles.MatchesProfiles
{
    public class MatchedProfile : Profile
    {
        public MatchedProfile() => CreateMap<UserInfoTable, MatchedResponse>();

    }
}
