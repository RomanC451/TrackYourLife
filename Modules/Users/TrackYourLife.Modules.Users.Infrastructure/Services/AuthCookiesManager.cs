using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.Modules.Users.Infrastructure.Options;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Infrastructure.Services;

public class AuthCookiesManager(
    IOptions<RefreshTokenCookieOptions> refreshTokenCookieOptions,
    IHttpContextAccessor httpContextAccessor
) : IAuthCookiesManager
{
    private readonly HttpContext? httpContext = httpContextAccessor.HttpContext;

    private CookieOptions CreateRefreshTokenCookieOptions() =>
        new()
        {
            Expires = DateTime.UtcNow.AddDays(refreshTokenCookieOptions.Value.DaysToExpire),
            HttpOnly = refreshTokenCookieOptions.Value.HttpOnly,
            IsEssential = refreshTokenCookieOptions.Value.IsEssential,
            Secure = refreshTokenCookieOptions.Value.Secure,
            SameSite = SameSiteMode.Unspecified,
            Domain = refreshTokenCookieOptions.Value.Domain
        };

    public Result<string> GetRefreshTokenFromCookie()
    {
        if (httpContext is null)
        {
            return Result.Failure<string>(InfrastructureErrors.HttpContext.NotExists);
        }

        var refreshTokenValue = httpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshTokenValue))
        {
            return Result.Failure<string>(TokenErrors.RefreshToken.NotExisting);
        }
        return Result.Success(refreshTokenValue);
    }

    public Result SetRefreshTokenCookie(Token refreshToken)
    {
        var cookieOptions = CreateRefreshTokenCookieOptions();

        cookieOptions.Expires = refreshToken.ExpiresAt;

        if (string.IsNullOrEmpty(refreshToken.Value))
        {
            return Result.Failure(TokenErrors.RefreshToken.Invalid);
        }

        if (httpContext is null)
        {
            return Result.Failure(InfrastructureErrors.HttpContext.NotExists);
        }

        httpContext.Response.Cookies.Append("refreshToken", refreshToken.Value, cookieOptions);

        return Result.Success();
    }

    public void DeleteRefreshTokenCookie()
    {
        var cookieOptions = CreateRefreshTokenCookieOptions();

        if (httpContext is null)
        {
            return;
        }

        cookieOptions.Expires = DateTime.UtcNow;

        httpContext.Response.Cookies.Delete("refreshToken", cookieOptions);
    }
}
