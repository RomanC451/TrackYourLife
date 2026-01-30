namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Time-based frequency data for workouts
/// </summary>
public sealed record WorkoutFrequencyDataDto(
    DateOnly Date,
    int WorkoutCount,
    DateOnly? StartDate = null,
    DateOnly? EndDate = null
);
