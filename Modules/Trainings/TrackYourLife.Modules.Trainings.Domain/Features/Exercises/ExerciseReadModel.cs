using System.Text.Json;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.Exercises;

public sealed record ExerciseReadModel(
    ExerciseId Id,
    UserId UserId,
    string Name,
    string? PictureUrl,
    string? VideoUrl,
    string? Description,
    string? Equipment,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc,
    string ExerciseSetsJson = "[]"
) : IReadModel<ExerciseId>
{
    public List<ExerciseSet> ExerciseSets
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(ExerciseSetsJson) ?? new();
        init => ExerciseSetsJson = JsonSerializer.Serialize(value);
    }
}
