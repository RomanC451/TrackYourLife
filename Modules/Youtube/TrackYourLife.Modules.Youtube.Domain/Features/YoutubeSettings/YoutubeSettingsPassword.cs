using System.Text.RegularExpressions;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;

public sealed partial class YoutubeSettingsPassword : ValueObject
{
    public const int MinLength = 10;

    private YoutubeSettingsPassword(string value) => Value = value;

    public string Value { get; }

    public static Result<YoutubeSettingsPassword> Create(string password) =>
        Result
            .Create(password)
            .Ensure(p => !string.IsNullOrEmpty(p.Trim()), YoutubeSettingsErrors.Password.Empty)
            .Ensure(p => p.Length >= MinLength, YoutubeSettingsErrors.Password.TooShort)
            .Ensure(p => LowerCaseRegex().Match(p).Success, YoutubeSettingsErrors.Password.LowerCase)
            .Ensure(p => UpperCaseRegex().Match(p).Success, YoutubeSettingsErrors.Password.UpperCase)
            .Ensure(p => NumberRegex().Match(p).Success, YoutubeSettingsErrors.Password.Number)
            .Ensure(
                p => SpecialCaracterRegex().Match(p).Success,
                YoutubeSettingsErrors.Password.SpecialCharacter
            )
            .Map(p => new YoutubeSettingsPassword(p));

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

    [GeneratedRegex("[@#$%^&+=!.]")]
    private static partial Regex SpecialCaracterRegex();
}
