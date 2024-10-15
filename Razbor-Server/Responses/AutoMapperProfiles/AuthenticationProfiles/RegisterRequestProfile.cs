using AutoMapper;
using RequestBodies.Authentication;

namespace Responses.AutoMapperProfiles.AuthenticationProfiles
{
    public class RegisterRequestProfile : Profile
    {
        public RegisterRequestProfile() => CreateMap<RegisterRequest, UserInfoTable>();
    }
}
