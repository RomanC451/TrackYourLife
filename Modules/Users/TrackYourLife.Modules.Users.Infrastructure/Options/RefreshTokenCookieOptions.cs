using Microsoft.AspNetCore.Http;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Users.Infrastructure.Options;

internal sealed class RefreshTokenCookieOptions : IOptions
{
    public const string ConfigurationSection = "RefreshTokenCookie";

    public int DaysToExpire { get; init; }

    public bool HttpOnly { get; init; }

    public bool IsEssential { get; init; }

    public bool Secure { get; init; }

    public string Domain { get; init; } = string.Empty;

    public SameSiteMode SameSite { get; init; } = SameSiteMode.Strict;
}
