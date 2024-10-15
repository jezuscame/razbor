
using AutoMapper;
using Responses.UserInformation;

namespace Responses.AutoMapperProfiles.UserInformationProfiles
{
    public class UserInfoProfile : Profile
    {
        public UserInfoProfile()
        {
            CreateMap<UserInfoTable, UserInfoResponse>();
        }
    }
}
