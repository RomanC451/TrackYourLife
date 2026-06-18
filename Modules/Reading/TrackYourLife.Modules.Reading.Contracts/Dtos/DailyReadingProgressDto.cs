namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record DailyReadingProgressDto(
    int? TargetPages,
    int PagesReadToday,
    int Remaining,
    bool TargetMet,
    bool HasTarget
);
