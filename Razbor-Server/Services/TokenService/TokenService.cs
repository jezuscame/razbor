using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Options.JwtOptions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.TokenService
{
    public class TokenService : ITokenService
    {
        public readonly JwtOptions _jwtOptions;
        public TokenService(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string Generate(UserInfoTable user)
        {
            var claims = new Claim[]
            {
                new(JwtRegisteredClaimNames.Sub, user.Username),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.GivenName, user.FirstName),
                new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                SecurityAlgorithms.HmacSha256
                );

            var token = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                null,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            string tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
    }
}
