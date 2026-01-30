namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Time-based aggregated value for workouts (duration or calories) per period.
/// </summary>
public sealed record WorkoutAggregatedValueDto(
    DateOnly Date,
    double Value,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null
);
