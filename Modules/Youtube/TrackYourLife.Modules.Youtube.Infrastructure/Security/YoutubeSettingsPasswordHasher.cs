using System.Security.Cryptography;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Security;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Security;

internal sealed class YoutubeSettingsPasswordHasher : IYoutubeSettingsPasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA256;
    private const char Delimiter = ';';

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName,
            KeySize
        );

        return string.Join(
            Delimiter,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash)
        );
    }

    public bool Verify(string passwordHash, string inputPassword)
    {
        var elements = passwordHash.Split(Delimiter);

        if (
            elements.Length != 2
            || string.IsNullOrWhiteSpace(elements[0])
            || string.IsNullOrWhiteSpace(elements[1])
        )
        {
            return false;
        }

        var salt = Convert.FromBase64String(elements[0]);
        var hash = Convert.FromBase64String(elements[1]);

        var hashInput = Rfc2898DeriveBytes.Pbkdf2(
            inputPassword,
            salt,
            Iterations,
            HashAlgorithmName,
            KeySize
        );

        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}
