using System.Text.Json.Serialization;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

/// <summary>
/// Enum representing the different types of exercise sets
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExerciseSetType
{
    /// <summary>
    /// Weight-based exercise set (reps + weight)
    /// </summary>
    Weight,

    /// <summary>
    /// Time-based exercise set (duration)
    /// </summary>
    Time,

    /// <summary>
    /// Bodyweight exercise set (reps only)
    /// </summary>
    Bodyweight,

    /// <summary>
    /// Distance-based exercise set (distance)
    /// </summary>
    Distance,
}
