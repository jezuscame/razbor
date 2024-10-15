using Microsoft.Extensions.Options;

namespace Services.Options.JwtOptions
{
    public class JwtOptionsConfiguration : IConfigureOptions<JwtOptions>
    {
        private readonly IConfiguration _configuration;
        public JwtOptionsConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void Configure(JwtOptions options)
        {
            _configuration.GetSection("Jwt").Bind(options);
        }
    }
}
