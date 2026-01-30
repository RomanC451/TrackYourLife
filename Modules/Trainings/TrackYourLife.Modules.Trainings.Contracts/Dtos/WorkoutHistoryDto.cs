using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;

namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Individual workout history entry
/// </summary>
public sealed record WorkoutHistoryDto(
    OngoingTrainingId Id,
    TrainingId TrainingId,
    string TrainingName,
    DateTime StartedOnUtc,
    DateTime FinishedOnUtc,
    long DurationSeconds,
    int? CaloriesBurned
);
