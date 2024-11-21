using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Services.Options.JwtOptions;
using Services.TokenService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;

namespace UnitTest.jwt
{
    public class TokenServiceTests
    {
        private TokenService _tokenService;
        private Mock<IOptions<JwtOptions>> _mockJwtOptions;
        private JwtOptions _jwtOptions;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Set up JwtOptions with mock data
            _jwtOptions = new JwtOptions
            {
                Issuer = "testIssuer",
                Audience = "testAudience",
                SecretKey = "superSecretKeyForSigningJWTThatIs256BitsLong!!" // 256-bit key
            };

            _mockJwtOptions = new Mock<IOptions<JwtOptions>>();
            _mockJwtOptions.Setup(opt => opt.Value).Returns(_jwtOptions);

            // Create the TokenService instance
            _tokenService = new TokenService(_mockJwtOptions.Object);
        }

        [Test]
        public void Generate_ShouldReturn_ValidJWTToken()
        {
            // Arrange
            var user = new UserInfoTable
            {
                Username = "testUser",
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User"
            };

            // Act
            var token = _tokenService.Generate(user);

            // ClassicAssert: Check if the token is not null or empty
            ClassicAssert.IsNotNull(token);
            ClassicAssert.IsNotEmpty(token);

            // Verify the token is a valid JWT token format
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            ClassicAssert.IsNotNull(jsonToken);

            // ClassicAssert claims are correct
            ClassicAssert.AreEqual("testUser", jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value);
            ClassicAssert.AreEqual("test@example.com", jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value);
            ClassicAssert.AreEqual("Test", jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value);
            ClassicAssert.AreEqual("User", jsonToken?.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.FamilyName)?.Value);

            // ClassicAssert the token's issuer and audience (use .Issuer and .Audience for JWT headers)
            ClassicAssert.AreEqual(_jwtOptions.Issuer, jsonToken?.Issuer);

            // ClassicAssert that the token is signed with the expected signing key
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        }
    }
}
