using AutoMapper;
using Responses.AutoMapperProfiles.AuthenticationProfiles;
using Responses.AutoMapperProfiles.PingPongProfiles;
using Responses.AutoMapperProfiles.UserInformationProfiles;
using Responses.AutoMapperProfiles.ChatProfiles;
using Responses.AutoMapperProfiles.MatchesProfiles;

namespace Responses.AutoMapperProfiles
{
    public static class AutoMapperConfiguration
    {
        public static void ConfigureAutoMapper(this IServiceCollection services) =>
            services.AddSingleton<IMapper>(CreateAutoMapperProfileConfiguration()
                .CreateMapper());
        

        private static MapperConfiguration CreateAutoMapperProfileConfiguration() =>
            new MapperConfiguration(config =>
            {
                config.AddProfile(new GetPingPongProfile());
                config.AddProfile(new RegisterRequestProfile());
                config.AddProfile(new UserInfoProfile());
                config.AddProfile(new ChatResponseProfile());
                config.AddProfile(new MatchedProfile());
            });
    }
}
