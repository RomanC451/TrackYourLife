using TrackYourLifeDotnet.Domain.Errors;
using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Domain.Shared;

namespace TrackYourLifeDotnet.Domain.ValueObjects;

public sealed class Name : ValueObject
{
    public const int MaxLength = 50;

    private Name(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Name> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<Name>(DomainErrors.Name.Empty);
        }

        if (firstName.Length > MaxLength)
        {
            return Result.Failure<Name>(DomainErrors.Name.TooLong);
        }

        return new Name(firstName);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
