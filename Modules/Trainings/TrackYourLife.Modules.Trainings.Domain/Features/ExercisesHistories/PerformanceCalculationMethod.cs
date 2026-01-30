namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

/// <summary>
/// Method for calculating exercise performance improvement
/// </summary>
public enum PerformanceCalculationMethod
{
    /// <summary>
    /// Sequential: Compare each workout to the previous one (workout 2 vs 1, 3 vs 2, etc.)
    /// </summary>
    Sequential = 0,

    /// <summary>
    /// FirstVsLast: Compare the first workout to the most recent one
    /// </summary>
    FirstVsLast = 1
}
