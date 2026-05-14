namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

public sealed record ExerciseStatsSummaryDto(
    double ImprovementDeltaPercent,
    double AverageVolumeInRange,
    double TotalVolumeInRange,
    int CompletedSessionsInRange,
    int SkippedSessionsInRange
);
