using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using Moq;
using TrackYourLife.Application.Core.Abstractions.Authentication;
using TrackYourLife.Domain.Users;
using TrackYourLife.Domain.Users.StrongTypes;
using TrackYourLife.Domain.Users.ValueObjects;
using TrackYourLife.Infrastructure.Authentication;
using TrackYourLife.Infrastructure.Options;

namespace TrackYourLife.Common.Infrastructure.UnitTests.Authentication;

public class JwtProviderTests
{
    private readonly JwtOptions _options;
    private readonly IJwtProvider _sut;

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

        _sut = new JwtProvider(optionsMock.Object);
    }

    [Fact]
    public void Generate_ShouldGenerateValidJwt()
    {
        // Arrange
        var user = User.Create(
            new UserId(Guid.NewGuid()),
            Email.Create("user@example.com").Value,
            new HashedPassword("asdasd"),
            Name.Create("aasdas").Value,
            Name.Create("asdsa").Value
        );

        // Act
        var jwt = _sut.Generate(user);

        // Assert
        Assert.NotEmpty(jwt);

        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwt);

        Assert.Equal(_options.Issuer, token.Issuer);
        Assert.Equal(_options.Audience, token.Audiences.First());
        Assert.Equal(user.Id.Value.ToString(), token.Subject);

        var emailClaim = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email);
        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email.Value, emailClaim.Value);

        var expiresAt = token.ValidTo;
        var expectedExpiresAt = DateTime.UtcNow.AddHours(1);
        Assert.InRange(
            expiresAt,
            expectedExpiresAt.AddSeconds(-2),
            expectedExpiresAt.AddSeconds(2)
        );
    }
}
