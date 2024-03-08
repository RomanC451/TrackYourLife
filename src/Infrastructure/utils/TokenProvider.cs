using System.Security.Cryptography;

namespace TrackYourLifeDotnet.Infrastructure.Utils;

public sealed class TokenProvider
{
    public static string Generate()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
