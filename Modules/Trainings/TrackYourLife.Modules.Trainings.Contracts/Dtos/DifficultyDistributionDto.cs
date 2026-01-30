namespace TrackYourLife.Modules.Trainings.Contracts.Dtos;

/// <summary>
/// Difficulty statistics and distribution
/// </summary>
public sealed record DifficultyDistributionDto(
    string Difficulty, // "Easy", "Medium", "Hard"
    int WorkoutCount,
    double Percentage
);
