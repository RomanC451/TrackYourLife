namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Main overview statistics for trainings module (totals and active training only).
/// Use separate endpoints for workout frequency, muscle group distribution, and difficulty distribution.
/// </summary>
public sealed record TrainingsOverviewDto(
    int TotalWorkoutsCompleted,
    long TotalTrainingTimeSeconds,
    int? TotalCaloriesBurned,
    bool HasActiveTraining
);
