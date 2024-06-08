using System.Text.RegularExpressions;
using TrackYourLife.Common.Domain.Errors;
using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

public sealed partial class Password : ValueObject
{
    public const int MinLength = 10;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Result<Password> Create(string password) =>
        Result
            .Create(password)
            .Ensure(p => !string.IsNullOrEmpty(p.Trim()), DomainErrors.Password.Empty)
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

    private static Regex LowerCaseRegex()
    {
        return new Regex("[a-z]");
    }

    private static Regex UpperCaseRegex()
    {
        return new Regex("[A-Z]");
    }

    private static Regex NumberRegex()
    {
        return new Regex("\\d");
    }

    private static Regex SpecialCaracterRegex()
    {
        return new Regex("[@#$%^&+=!.]");
    }
}
