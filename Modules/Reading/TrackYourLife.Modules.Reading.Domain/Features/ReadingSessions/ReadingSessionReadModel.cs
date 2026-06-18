using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public sealed record ReadingSessionReadModel(
    ReadingSessionId Id,
    UserId UserId,
    BookId BookId,
    DateOnly? SessionDate,
    int StartPage,
    int? EndPage,
    int? PagesRead,
    int? DurationSeconds,
    string? Notes,
    DateTime StartedOnUtc,
    DateTime? FinishedOnUtc,
    DateTime CreatedOnUtc
) : IReadModel<ReadingSessionId>
{
    public string BookTitle { get; init; } = string.Empty;

    public string BookAuthor { get; init; } = string.Empty;

    public bool IsActive => !FinishedOnUtc.HasValue;
}
