using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.SharedLib.Domain.Results;

public interface IValidationResult
{
    public static readonly Error ValidationError = new(
        "ValidationError",
        "A validation problem occurred.",
        400
    );

    Error[] Errors { get; }
}
