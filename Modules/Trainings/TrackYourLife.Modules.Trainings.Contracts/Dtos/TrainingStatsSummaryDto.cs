namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

public sealed record TrainingStatsSummaryDto(
    int SessionsCompleted,
    int FullyCompletedCount,
    int WithSkippedCount,
    double CompletionRate,
    double AverageDurationSeconds,
    double TotalDurationSeconds,
    int? AverageCaloriesBurned,
    int? TotalCaloriesBurned,
    DateTime? LastPerformedOnUtc,
    DateOnly WindowStartDate,
    DateOnly WindowEndDate
);
