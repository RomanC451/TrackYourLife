using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Training template usage statistics
/// </summary>
public sealed record TrainingTemplateUsageDto(
    TrainingId TrainingId,
    string TrainingName,
    int TotalCompleted,
    int TotalFullyCompleted,
    int TotalWithSkippedExercises,
    double CompletionRate,
    double AverageDurationSeconds,
    int? AverageCaloriesBurned
);
