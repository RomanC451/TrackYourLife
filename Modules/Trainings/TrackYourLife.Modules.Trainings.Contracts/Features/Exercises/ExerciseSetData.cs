namespace TrackYourLife.Modules.Trainings.Contracts.Features.Exercises;

/// <summary>
/// Flattened DTO for exercise sets with generic count/unit properties
/// </summary>
public sealed record ExerciseSetData
{
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

    /// <summary>
    /// First count value (required)
    /// </summary>
    public float Count1 { get; set; }

    /// <summary>
    /// First unit value (required)
    /// </summary>
    public string Unit1 { get; set; } = string.Empty;

    /// <summary>
    /// Second count value (optional)
    /// </summary>
    public float? Count2 { get; set; }

    /// <summary>
    /// Second unit value (optional)
    /// </summary>
    public string? Unit2 { get; set; }
}
