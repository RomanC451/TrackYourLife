using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Exercise performance data with aggregated improvement percentage per exercise
/// </summary>
public sealed record ExercisePerformanceDto(
    ExerciseId ExerciseId,
    string ExerciseName,
    double ImprovementPercentage // Calculated improvement percentage based on volume (aggregated per exercise)
);
