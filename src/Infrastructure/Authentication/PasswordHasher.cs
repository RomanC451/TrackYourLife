using System.Security.Cryptography;
using TrackYourLifeDotnet.Application.Abstractions.Authentication;
using TrackYourLifeDotnet.Domain.Users.ValueObjects;

namespace TrackYourLifeDotnet.Infrastructure.Authentication;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 128 / 8;
    private const int KeySize = 256 / 8;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName _hashAlgorithmName = HashAlgorithmName.SHA256;
    private const char Delimiter = ';';

    public HashedPassword Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            _hashAlgorithmName,
            KeySize
        );

        var passwordHash = string.Join(
            Delimiter,
            Convert.ToBase64String(salt),
            Convert.ToBase64String(hash)
        );

        return new HashedPassword(passwordHash);
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
            // The hash format is invalid, so return false.
            return false;
        }

        var salt = Convert.FromBase64String(elements[0]);
        var hash = Convert.FromBase64String(elements[1]);

        var hashInput = Rfc2898DeriveBytes.Pbkdf2(
            inputPassword,
            salt,
            Iterations,
            _hashAlgorithmName,
            KeySize
        );

        return CryptographicOperations.FixedTimeEquals(hash, hashInput);
    }
}
