using Microsoft.AspNetCore.Http;
using TrackYourLife.Modules.Users.Domain.Tokens;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Services;

public interface IAuthCookiesManager
{
    Result<string> GetRefreshTokenFromCookie();

    Result SetRefreshTokenCookie(Token refreshToken);

    void DeleteRefreshTokenCookie();
}
