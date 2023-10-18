using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using TrackYourLifeDotnet.Infrastructure.Options;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;
using Xunit;

namespace TrackYourLifeDotnet.Tests.Infrastructure.OptionsSetup
{
    public class JwtBearerOptionsSetupTests
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_WhenJwtOptionsIsNull()
        {
            // Arrange
            IOptions<JwtOptions> jwtOptions = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new JwtBearerOptionsSetup(jwtOptions));
        }

        [Fact]
        public void PostConfigure_ThrowsArgumentNullException_WhenOptionsIsNull()
        {
            // Arrange
            var jwtOptions = new JwtOptions();
            var options = (JwtBearerOptions)null!;
            var postConfigure = new JwtBearerOptionsSetup(Options.Create(jwtOptions));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => postConfigure.PostConfigure(null, options));
        }

        [Fact]
        public void PostConfigure_SetsTokenValidationParameters()
        {
            // Arrange
            var jwtOptions = new JwtOptions
            {
                Issuer = "test-issuer",
                Audience = "test-audience",
                SecretKey = "test-secret-key"
            };

            var options = new JwtBearerOptions();
            var postConfigure = new JwtBearerOptionsSetup(Options.Create(jwtOptions));

            // Act
            postConfigure.PostConfigure(null, options);

            // Assert
            Assert.Equal(jwtOptions.Issuer, options.TokenValidationParameters.ValidIssuer);
            Assert.Equal(jwtOptions.Audience, options.TokenValidationParameters.ValidAudience);
            Assert.Equal(Encoding.UTF8.GetBytes(jwtOptions.SecretKey), ((SymmetricSecurityKey)options.TokenValidationParameters.IssuerSigningKey).Key);
        }
    }
}