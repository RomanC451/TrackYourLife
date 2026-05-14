namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

public sealed record ExerciseConsistencyPointDto(
    DateOnly WeekStartDate,
    int CompletedSessionsCount,
    int SkippedSessionsCount
);
