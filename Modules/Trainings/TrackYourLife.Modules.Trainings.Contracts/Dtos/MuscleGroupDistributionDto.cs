namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Muscle group statistics and distribution
/// </summary>
public sealed record MuscleGroupDistributionDto(
    string MuscleGroup,
    int WorkoutCount,
    double Percentage
);
