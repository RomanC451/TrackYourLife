using System.Security.Cryptography;

namespace TrackYourLifeDotnet.Infrastructure.utils;

public sealed class TokenProvider
{
    public static string Generate()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }
}
