using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrackYourLifeDotnet.Infrastructure.Authentication;
using TrackYourLifeDotnet.Infrastructure.OptionsSetup;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.OptionsSetup;

public class JwtBearerOptionsSetupTests
{
    [Fact]
    public void PostConfigure_SetsTokenValidationParameters()
    {
        // Arrange
        var jwtOptions = new JwtOptions
        {
            Issuer = "testissuer",
            Audience = "testaudience",
            SecretKey = "testkey"
        };
        var options = new JwtBearerOptions();
        var postConfigurator = new JwtBearerOptionsSetup(Options.Create(jwtOptions));

        // Act
        postConfigurator.PostConfigure(null, options);

        // Assert
        Assert.Equal(jwtOptions.Issuer, options.TokenValidationParameters.ValidIssuer);
        Assert.Equal(jwtOptions.Audience, options.TokenValidationParameters.ValidAudience);
        Assert.Equal(
            jwtOptions.SecretKey,
            Encoding.UTF8.GetString(
                ((SymmetricSecurityKey)options.TokenValidationParameters.IssuerSigningKey)?.Key
                    ?? Array.Empty<byte>()
            )
        );
    }

    [Fact]
    public void PostConfigure_WithNullOptions_ThrowsException()
    {
        // Arrange
        var jwtOptions = new JwtOptions
        {
            Issuer = "testissuer",
            Audience = "testaudience",
            SecretKey = "testkey"
        };
        var postConfigurator = new JwtBearerOptionsSetup(Options.Create(jwtOptions));

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => postConfigurator.PostConfigure(null, null!));
    }
}
