namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record ReadingStreakDayDto(DateOnly Date, bool TargetMet, int PagesRead);

public sealed record ReadingStreakDto(
    int CurrentStreak,
    int LongestStreak,
    bool TodayTargetMet,
    IReadOnlyList<ReadingStreakDayDto> SuccessfulDays
);
