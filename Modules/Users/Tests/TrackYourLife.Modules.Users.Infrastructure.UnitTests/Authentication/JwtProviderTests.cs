using System.IdentityModel.Tokens.Jwt;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Domain.Features.Users;
using TrackYourLife.Modules.Users.Infrastructure.Authentication;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Authentication;

public class JwtProviderTests
{
    private readonly IJwtProvider _jwtProvider;
    private readonly JwtOptions _jwtOptions;

    public JwtProviderTests()
    {
        _jwtOptions = new JwtOptions
        {
            SecretKey = "super-secret-key-that-is-at-least-16-characters",
            Issuer = "test-issuer",
            Audience = "test-audience",
            MinutesToExpire = 60,
        };

        var options = Microsoft.Extensions.Options.Options.Create(_jwtOptions);
        _jwtProvider = new JwtProvider(options);
    }

    [Fact]
    public async Task Generate_WithValidUser_ReturnsValidJwtToken()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var user = new UserReadModel(
            userId,
            "Test",
            "User",
            "test@example.com",
            "testuser",
            DateTime.UtcNow,
            PlanType.Free,
            null,
            null,
            null
        );

        // Act
        var token = await _jwtProvider.GenerateAsync(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken.Should().NotBeNull();
        jwtToken.Issuer.Should().Be(_jwtOptions.Issuer);
        jwtToken.Audiences.Should().Contain(_jwtOptions.Audience);
    }

    [Fact]
    public async Task Generate_WithValidUser_ContainsCorrectClaims()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var user = new UserReadModel(
            userId,
            "Test",
            "User",
            "test@example.com",
            "testuser",
            DateTime.UtcNow,
            PlanType.Free,
            null,
            null,
            null
        );

        // Act
        var token = await _jwtProvider.GenerateAsync(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        var claims = jwtToken.Claims.ToList();

        claims
            .Should()
            .Contain(c =>
                c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.Value.ToString()
            );
        claims
            .Should()
            .Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
    }

    [Fact]
    public async Task Generate_WithValidUser_TokenExpiresAfterConfiguredTime()
    {
        // Arrange
        var userId = UserId.Create(Guid.NewGuid());
        var user = new UserReadModel(
            userId,
            "Test",
            "User",
            "test@example.com",
            "testuser",
            DateTime.UtcNow,
            PlanType.Free,
            null,
            null,
            null
        );

        // Act
        var token = await _jwtProvider.GenerateAsync(user);

        // Assert
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadJwtToken(token);

        jwtToken
            .ValidTo.Should()
            .BeCloseTo(
                DateTime.UtcNow.AddMinutes(_jwtOptions.MinutesToExpire),
                TimeSpan.FromSeconds(2)
            );
    }
}
