using System;
using TrackYourLife.Common.Domain.Shared;

namespace TrackYourLife.Common.Domain.Utility;

public class Ensure
{
    public static Result IsTrue(bool condition, Error error)
    {
        if (!condition)
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
}
