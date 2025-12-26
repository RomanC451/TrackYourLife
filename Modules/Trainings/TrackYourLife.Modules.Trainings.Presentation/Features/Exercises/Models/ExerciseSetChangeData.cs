using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

namespace TrackYourLife.Modules.Trainings.Presentation.Features.Exercises.Models;

/// <summary>
/// Flattened DTO for exercise set changes with explicit type field and validation
/// </summary>
public sealed record ExerciseSetChangeData
{
    /// <summary>
    /// The ID of the exercise set being changed
    /// </summary>
    public Guid SetId { get; set; }

    /// <summary>
    /// The type of exercise set change
    /// </summary>
    public ExerciseSetType Type { get; set; }

    // Weight-based change properties
    public float? WeightChange { get; set; }
    public int? RepsChange { get; set; }

    // Time-based change properties
    public int? DurationChangeSeconds { get; set; }

    // Distance-based change properties
    public float? DistanceChange { get; set; }
}
