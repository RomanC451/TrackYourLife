using System.Text.RegularExpressions;
using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.ValueObjects;

public sealed partial class Password : ValueObject
{
    public const int MinLength = 8;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Result<Password> Create(string password) =>
        Result
            .Create(password.Trim())
            .Ensure(p => !string.IsNullOrEmpty(p), DomainErrors.Password.Empty)
            .Ensure(p => p.Length >= MinLength, DomainErrors.Password.TooShort)
            .Ensure(p => LowerCaseRegex().Match(p).Success, DomainErrors.Password.LowerCase)
            .Ensure(p => UpperCaseRegex().Match(p).Success, DomainErrors.Password.UpperCase)
            .Ensure(p => NumberRegex().Match(p).Success, DomainErrors.Password.Number)
            .Ensure(
                p => SpecialCaracterRegex().Match(p).Success,
                DomainErrors.Password.SpecialCharacter
            )
            .Map(p => new Password(p));

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }

    [GeneratedRegex("[a-z]")]
    private static partial Regex LowerCaseRegex();

    [GeneratedRegex("[A-Z]")]
    private static partial Regex UpperCaseRegex();

    [GeneratedRegex("\\d")]
    private static partial Regex NumberRegex();

    [GeneratedRegex("[@$!%*?&.]")]
    private static partial Regex SpecialCaracterRegex();
}
