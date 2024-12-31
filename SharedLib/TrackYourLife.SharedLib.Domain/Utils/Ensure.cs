using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.SharedLib.Domain.Utils;

public static class Ensure
{
    public static Result IsTrue(bool condition, Error error)
    {
        if (!condition)
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }

    public static Result IsFalse(bool condition, Error error)
    {
        if (condition)
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }

    public static Result IsInEnum<T>(T value, Error error)
        where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }

    public static Result NotNull<T>(T value, Error error)
    {
        if (value is null)
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }

    public static Result NotEmptyList<T>(IEnumerable<T> collection, Error error)
    {
        if (collection == null || !collection.Any())
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }

    public static Result NotEmpty(string value, Error error)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }

    public static Result NotEmpty<T>(T value, Error Error)
    {
        if (value is null || value.Equals(default(T)))
        {
            return Result.Failure(Error);
        }

        return Result.Success();
    }

    public static Result NotEmptyId<T>(T id, Error Error)
        where T : IStronglyTypedGuid
    {
        if (id.Value == Guid.Empty)
        {
            return Result.Failure(Error);
        }

        return Result.Success();
    }

    public static Result NotNegative(float value, Error error)
    {
        if (value < 0)
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }

    public static Result Positive(float value, Error error)
    {
        if (value <= 0)
        {
            return Result.Failure(error);
        }

        return Result.Success();
    }
}
