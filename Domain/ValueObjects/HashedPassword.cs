using System.Text.RegularExpressions;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.ValueObjects;

public sealed class PasswordHash : ValueObject
{
    private PasswordHash(string value) => Value = value;

    public string Value { get; }

    public static Result<PasswordHash> Create(string passwordHash) =>
        Result.Create(passwordHash).Map(p => new PasswordHash(p));

    public static Result<PasswordHash> CreateHash(string password, Func<string, string> hashFunc)
    {
        Result<Password> passwordResult = Password.Create(password);
        if (passwordResult.IsFailure)
        {
            return Result.Failure<PasswordHash>(passwordResult.Error);
        }

        string hash = hashFunc(passwordResult.Value.Value);

        return Result.Create(hash).Map(p => new PasswordHash(p));
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
