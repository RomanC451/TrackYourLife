using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Moq;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Domain.Entities;
using TrackYourLifeDotnet.Domain.ValueObjects;
using TrackYourLifeDotnet.Infrastructure.Authentication;

namespace TrackYourLifeDotnet.Infrastructure.UnitTests.Authentication;

public class JwtProviderTests
{
    private readonly JwtOptions _options;
    private readonly IJwtProvider _jwtProvider;

    public JwtProviderTests()
    {
        _options = new JwtOptions
        {
            SecretKey = "mySecretKey super key",
            Issuer = "myIssuer",
            Audience = "myAudience"
        };

        var optionsMock = new Mock<IOptions<JwtOptions>>();
        optionsMock.SetupGet(x => x.Value).Returns(_options);

        _jwtProvider = new JwtProvider(optionsMock.Object);
    }

    [Fact]
    public void Generate_ShouldGenerateValidJwt()
    {
        // Arrange
        var user = User.Create(
            Guid.NewGuid(),
            Email.Create("user@example.com").Value,
            PasswordHash.Create("asdasd").Value,
            FirstName.Create("aasdas").Value,
            LastName.Create("asdsa").Value
        );

        // Act
        var jwt = _jwtProvider.Generate(user);

        // Assert
        Assert.NotEmpty(jwt);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        Assert.Equal(_options.Issuer, token.Issuer);
        Assert.Equal(_options.Audience, token.Audiences.First());
        Assert.Equal(user.Id.ToString(), token.Subject);

        var emailClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email.Value, emailClaim.Value);

        var expiresAt = token.ValidTo;
        var expectedExpiresAt = DateTime.UtcNow.AddHours(1);
        Assert.InRange(
            expiresAt,
            expectedExpiresAt.AddSeconds(-1),
            expectedExpiresAt.AddSeconds(1)
        );
    }
}
