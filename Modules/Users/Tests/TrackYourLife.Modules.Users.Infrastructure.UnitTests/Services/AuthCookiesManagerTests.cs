using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Users.Domain.Features.Tokens;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.Modules.Users.Infrastructure.Services;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Infrastructure.UnitTests.Services;

public class AuthCookiesManagerTests
{
    private readonly IOptions<RefreshTokenCookieOptions> _refreshTokenCookieOptions;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AuthCookiesManager _authCookiesManager;
    private readonly HttpContext _httpContext;
    private readonly HttpResponse _response;
    private readonly HttpRequest _request;
    private readonly UserId _testUserId;

    public AuthCookiesManagerTests()
    {
        _refreshTokenCookieOptions = Microsoft.Extensions.Options.Options.Create(
            new RefreshTokenCookieOptions
            {
                DaysToExpire = 7,
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                Domain = "test.com",
            }
        );

        _httpContext = Substitute.For<HttpContext>();
        _response = Substitute.For<HttpResponse>();
        _request = Substitute.For<HttpRequest>();
        _httpContext.Response.Returns(_response);
        _httpContext.Request.Returns(_request);

        _httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        _httpContextAccessor.HttpContext.Returns(_httpContext);

        _testUserId = UserId.Create(Guid.NewGuid());

        _authCookiesManager = new AuthCookiesManager(
            _refreshTokenCookieOptions,
            _httpContextAccessor
        );
    }

    [Fact]
    public void GetRefreshTokenFromCookie_ValidToken_ShouldReturnSuccess()
    {
        // Arrange
        const string refreshToken = "valid-refresh-token";
        _request.Cookies["refreshToken"].Returns(refreshToken);

        // Act
        var result = _authCookiesManager.GetRefreshTokenFromCookie();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(refreshToken);
    }

    [Fact]
    public void GetRefreshTokenFromCookie_NoHttpContext_ShouldReturnFailure()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        var tmpAuthCookiesManager = new AuthCookiesManager(
            _refreshTokenCookieOptions,
            _httpContextAccessor
        );

        // Act
        var result = tmpAuthCookiesManager.GetRefreshTokenFromCookie();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.HttpContext.NotExists);
    }

    [Fact]
    public void GetRefreshTokenFromCookie_NoToken_ShouldReturnFailure()
    {
        // Arrange
        _request.Cookies["refreshToken"].Returns((string?)null);

        // Act
        var result = _authCookiesManager.GetRefreshTokenFromCookie();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(TokenErrors.RefreshToken.NotExisting);
    }

    [Fact]
    public void SetRefreshTokenCookie_ValidToken_ShouldSetCookie()
    {
        // Arrange
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var tokenResult = Token.Create(
            TokenId.Create(Guid.NewGuid()),
            "valid-token",
            _testUserId,
            TokenType.RefreshToken,
            expiresAt
        );
        var refreshToken = tokenResult.Value;

        // Act
        var result = _authCookiesManager.SetRefreshTokenCookie(refreshToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _response
            .Received(1)
            .Cookies.Append(
                "refreshToken",
                refreshToken.Value,
                Arg.Is<CookieOptions>(options =>
                    options.Expires == refreshToken.ExpiresAt
                    && options.HttpOnly == _refreshTokenCookieOptions.Value.HttpOnly
                    && options.Secure == _refreshTokenCookieOptions.Value.Secure
                    && options.Domain == _refreshTokenCookieOptions.Value.Domain
                )
            );
    }

    [Fact]
    public void SetRefreshTokenCookie_NoHttpContext_ShouldReturnFailure()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var expiresAt = DateTime.UtcNow.AddDays(7);
        var tokenResult = Token.Create(
            TokenId.Create(Guid.NewGuid()),
            "valid-token",
            _testUserId,
            TokenType.RefreshToken,
            expiresAt
        );
        var refreshToken = tokenResult.Value;

        var authCookiesManager = new AuthCookiesManager(
            _refreshTokenCookieOptions,
            _httpContextAccessor
        );

        // Act
        var result = authCookiesManager.SetRefreshTokenCookie(refreshToken);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(InfrastructureErrors.HttpContext.NotExists);
    }

    [Fact]
    public void DeleteRefreshTokenCookie_ShouldDeleteCookie()
    {
        // Act
        _authCookiesManager.DeleteRefreshTokenCookie();

        // Assert
        _response
            .Received(1)
            .Cookies.Delete(
                "refreshToken",
                Arg.Is<CookieOptions>(options =>
                    options.Expires <= DateTime.UtcNow
                    && options.HttpOnly == _refreshTokenCookieOptions.Value.HttpOnly
                    && options.Secure == _refreshTokenCookieOptions.Value.Secure
                    && options.Domain == _refreshTokenCookieOptions.Value.Domain
                )
            );
    }

    [Fact]
    public void DeleteRefreshTokenCookie_NoHttpContext_ShouldNotThrow()
    {
        // Arrange
        _httpContextAccessor.HttpContext.Returns((HttpContext?)null);

        // Act & Assert
        var exception = Record.Exception(() => _authCookiesManager.DeleteRefreshTokenCookie());
        exception.Should().BeNull();
    }
}
