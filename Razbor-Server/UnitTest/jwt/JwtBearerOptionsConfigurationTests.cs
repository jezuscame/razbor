using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Services.Options.JwtOptions;
using System.Text;

namespace UnitTest.jwt
{
    public class JwtBearerOptionsConfigurationTests
    {
        private JwtBearerOptionsConfiguration _jwtBearerOptionsConfiguration;
        private Mock<IOptions<JwtOptions>> _mockOptions;
        private JwtOptions _jwtOptions;

        [SetUp]
        public void SetUp()
        {
            // Arrange: Setup the mock JwtOptions
            _jwtOptions = new JwtOptions
            {
                Issuer = "myIssuer",
                Audience = "myAudience",
                SecretKey = "mySuperSecretKey123!"
            };

            _mockOptions = new Mock<IOptions<JwtOptions>>();
            _mockOptions.Setup(opt => opt.Value).Returns(_jwtOptions);

            // Create the JwtBearerOptionsConfiguration instance
            _jwtBearerOptionsConfiguration = new JwtBearerOptionsConfiguration(_mockOptions.Object);
        }

        [Test]
        public void Configure_ShouldSetTokenValidationParameters()
        {
            // Arrange
            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            _jwtBearerOptionsConfiguration.Configure(jwtBearerOptions);

            // Assert
            var tokenValidationParameters = jwtBearerOptions.TokenValidationParameters;

            ClassicAssert.IsNotNull(tokenValidationParameters);
            ClassicAssert.IsTrue(tokenValidationParameters.ValidateIssuer);
            ClassicAssert.IsTrue(tokenValidationParameters.ValidateAudience);
            ClassicAssert.IsTrue(tokenValidationParameters.ValidateLifetime);
            ClassicAssert.IsTrue(tokenValidationParameters.ValidateIssuerSigningKey);
            ClassicAssert.AreEqual(_jwtOptions.Issuer, tokenValidationParameters.ValidIssuer);
            ClassicAssert.AreEqual(_jwtOptions.Audience, tokenValidationParameters.ValidAudience);

            // Validate IssuerSigningKey
            var expectedSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var actualSigningKey = tokenValidationParameters.IssuerSigningKey as SymmetricSecurityKey;
            ClassicAssert.IsNotNull(actualSigningKey, "IssuerSigningKey should be of type SymmetricSecurityKey.");
            ClassicAssert.AreEqual(expectedSigningKey.Key, actualSigningKey.Key);
        }

        [Test]
        public void Configure_ShouldUseCorrectIssuerSigningKey()
        {
            // Arrange
            var jwtBearerOptions = new JwtBearerOptions();

            // Act
            _jwtBearerOptionsConfiguration.Configure(jwtBearerOptions);

            // Assert
            var tokenValidationParameters = jwtBearerOptions.TokenValidationParameters;
            var expectedSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var actualSigningKey = tokenValidationParameters.IssuerSigningKey as SymmetricSecurityKey;

            ClassicAssert.IsNotNull(actualSigningKey, "IssuerSigningKey should be of type SymmetricSecurityKey.");
            ClassicAssert.AreEqual(expectedSigningKey.Key, actualSigningKey.Key);
        }
    }
}
