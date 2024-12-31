using System.Text.RegularExpressions;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

public sealed partial class Password : ValueObject
{
    public const int MinLength = 10;

    private Password(string value) => Value = value;

    public string Value { get; }

    public static Result<Password> Create(string password) =>
        Result
            .Create(password)
            .Ensure(p => !string.IsNullOrEmpty(p.Trim()), UserErrors.Password.Empty)
            .Ensure(p => p.Length >= MinLength, UserErrors.Password.TooShort)
            .Ensure(p => LowerCaseRegex().Match(p).Success, UserErrors.Password.LowerCase)
            .Ensure(p => UpperCaseRegex().Match(p).Success, UserErrors.Password.UpperCase)
            .Ensure(p => NumberRegex().Match(p).Success, UserErrors.Password.Number)
            .Ensure(
                p => SpecialCaracterRegex().Match(p).Success,
                UserErrors.Password.SpecialCharacter
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
