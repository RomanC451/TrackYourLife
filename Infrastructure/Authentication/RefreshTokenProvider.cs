using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;

namespace TrackYourLifeDotnet.Infrastructure.Authentication;

public sealed class RefreshTokenProvider : IRefreshTokenProvider
{
    public string Generate()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public static CookieOptions GetCookieOptions(DateTime expiresAt) =>
        new() { HttpOnly = true, Expires = expiresAt };
}
