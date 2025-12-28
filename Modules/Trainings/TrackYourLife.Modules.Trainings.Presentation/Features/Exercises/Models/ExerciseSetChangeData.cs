namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

/// <summary>
/// Flattened DTO for exercise set changes with generic count/unit properties
/// </summary>
public sealed record ExerciseSetChangeData
{
    /// <summary>
    /// The ID of the exercise set being changed
    /// </summary>
    public Guid SetId { get; set; }

    /// <summary>
    /// Change to Count1
    /// </summary>
    public float? Count1Change { get; set; }

    /// <summary>
    /// Change to Unit1
    /// </summary>
    public string? Unit1Change { get; set; }

    /// <summary>
    /// Change to Count2
    /// </summary>
    public float? Count2Change { get; set; }

    /// <summary>
    /// Change to Unit2
    /// </summary>
    public string? Unit2Change { get; set; }
}
