using System.Security.Cryptography;

namespace TrackYourLife.Common.Infrastructure.Utils;

public sealed class TokenProvider
{
    public static string Generate(int bytes = 32)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytes));
    }
}
