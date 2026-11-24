using System.Security.Cryptography;
using System.Text;
using TrackYourLife.Modules.Users.Application.Core.Abstraction.Authentication;
using TrackYourLife.Modules.Users.Domain.Features.PersonalAccessTokens;

namespace TrackYourLife.Modules.Users.Infrastructure.Authentication;

internal sealed class PersonalAccessTokenHasher : IPersonalAccessTokenHasher
{
    public string GenerateRawToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var payload = Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        return $"{PersonalAccessToken.TokenPrefixMarker}{payload}";
    }

    public string Hash(string rawToken)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    public string GetVisiblePrefix(string rawToken)
    {
        if (
            !rawToken.StartsWith(PersonalAccessToken.TokenPrefixMarker, StringComparison.Ordinal)
            || rawToken.Length
                < PersonalAccessToken.TokenPrefixMarker.Length
                    + PersonalAccessToken.VisiblePrefixLength
        )
        {
            return rawToken[
                ..Math.Min(
                    PersonalAccessToken.TokenPrefixMarker.Length
                        + PersonalAccessToken.VisiblePrefixLength,
                    rawToken.Length
                )
            ];
        }

        return rawToken[
            ..(
                PersonalAccessToken.TokenPrefixMarker.Length
                + PersonalAccessToken.VisiblePrefixLength
            )
        ];
    }
}
