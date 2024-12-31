using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Domain.Users.ValueObjects;

public sealed class Email : ValueObject
{
    public const int MaxLength = 255;

    private Email(string value) => Value = value;

    public string Value { get; }

    public static Result<Email> Create(string email) =>
        Result
            .Create(email)
            .Ensure(e => !string.IsNullOrWhiteSpace(e.Trim()), UserErrors.Email.Empty)
            .Ensure(e => e.Trim().Length <= MaxLength, UserErrors.Email.TooLong)
            .Ensure(e => e.Trim().Split('@').Length == 2, UserErrors.Email.InvalidFormat)
            .Map(e => new Email(e.Trim()));

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
