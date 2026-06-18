namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record ReadingDashboardDto(
    ReadingStreakDto Streak,
    DailyReadingProgressDto DailyProgress,
    ReadingSessionDto? ActiveSession,
    IReadOnlyList<BookDto> RecentBooks,
    IReadOnlyList<BookNoteDto> RecentNotes
);
