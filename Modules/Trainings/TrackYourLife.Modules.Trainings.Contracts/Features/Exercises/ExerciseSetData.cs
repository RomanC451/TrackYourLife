using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Contracts.Features.Exercises;

/// <summary>
/// Flattened DTO for exercise sets with explicit type field and validation
/// </summary>
public sealed record ExerciseSetData
{
    /// <summary>
    /// The type of exercise set
    /// </summary>
    public ExerciseSetType Type { get; set; }

    /// <summary>
    /// Name of the exercise set
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Order index for the set
    /// </summary>
    public int OrderIndex { get; set; }

    /// <summary>
    /// Rest time after this set in seconds
    /// </summary>
    public int? RestTime { get; set; }

    // Weight-based properties
    public int? Reps { get; set; }
    public float? Weight { get; set; }

    // Time-based properties
    public int? Duration { get; set; }

    // Distance-based properties
    public float? Distance { get; set; }

    // Custom properties
    public string? CustomValue { get; set; }
    public string? CustomUnit { get; set; }
}
