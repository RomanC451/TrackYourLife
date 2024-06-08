using TrackYourLife.Domain.Shared;

namespace TrackYourLife.Common.Presentation.Contracts;

public sealed class ApiErrorResponse(IReadOnlyCollection<Error> errors)
{
    public IReadOnlyCollection<Error> Errors { get; } = errors;
}
