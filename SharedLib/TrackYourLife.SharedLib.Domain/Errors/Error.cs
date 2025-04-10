using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Domain.Errors;

public class Error : IEquatable<Error>
{
    public static readonly Error None = new(string.Empty, string.Empty, 0);
    public static readonly Error NullValue = new(
        "Error.NullValue",
        "The specified result value is null."
    );

    public static readonly Func<IStronglyTypedGuid, string, Error> NotFound = (id, entityName) =>
        new($"{entityName}.NotFound", $"{entityName} with id {id} was not found.", 404);

    public static readonly Func<IStronglyTypedGuid, string, Error> NotOwned = (id, entityName) =>
        new Error(
            $"{entityName}.NotOwned",
            $"{entityName} with id {id} is not owned by the user.",
            403
        );

    public Error(string code, string message, int httpStatus = 400)
    {
        Code = code;
        Message = message;
        HttpStatus = httpStatus;
    }

    public string Code { get; }

    public string Message { get; }

    public int HttpStatus { get; }

    public static implicit operator string(Error error) => error.Code;

    public static bool operator ==(Error? a, Error? b)
    {
        if (a is null && b is null)
        {
            return true;
        }

        if (a is null || b is null)
        {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Error? a, Error? b) => !(a == b);

    public virtual bool Equals(Error? other)
    {
        if (other is null)
        {
            return false;
        }

        return Code == other.Code && Message == other.Message;
    }

    public override bool Equals(object? obj) => obj is Error error && Equals(error);

    public override int GetHashCode() => HashCode.Combine(Code, Message);

    public override string ToString() => Code;
}
