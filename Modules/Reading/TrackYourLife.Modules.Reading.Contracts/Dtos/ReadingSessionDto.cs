namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record ReadingSessionDto(
    Guid Id,
    Guid BookId,
    string BookTitle,
    string BookAuthor,
    DateOnly? SessionDate,
    int StartPage,
    int? EndPage,
    int? PagesRead,
    int? DurationSeconds,
    DateTime StartedOnUtc,
    DateTime? FinishedOnUtc,
    DateTime CreatedAt,
    bool IsActive
);
