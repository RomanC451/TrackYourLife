using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Most performed exercises statistics
/// </summary>
public sealed record TopExerciseDto(
    ExerciseId ExerciseId,
    string ExerciseName,
    int CompletionCount,
    int SkipCount,
    double ImprovementPercentage
);
