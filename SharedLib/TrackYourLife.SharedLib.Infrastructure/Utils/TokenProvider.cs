using System.Security.Cryptography;

namespace TrackYourLife.SharedLib.Infrastructure.Utils;

public static class TokenProvider
{
    public static string Generate(int bytes = 32)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(bytes));
    }
}
