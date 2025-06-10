using System.Text.Json;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public sealed record ExerciseHistoryReadModel(
    ExerciseHistoryId Id,
    ExerciseId ExerciseId,
    string ExerciseSetChangesJson = "[]",
    string ExerciseSetsBeforeChangeJson = "[]",
    bool AreChangesApplied = false,
    DateTime CreatedOnUtc = default,
    DateTime? ModifiedOnUtc = null
) : IReadModel<ExerciseHistoryId>
{
    public IReadOnlyList<ExerciseSetChange> ExerciseSetChanges
    {
        get => JsonSerializer.Deserialize<List<ExerciseSetChange>>(ExerciseSetChangesJson) ?? [];
        init => ExerciseSetChangesJson = JsonSerializer.Serialize(value);
    }

    public IReadOnlyList<ExerciseSet> ExerciseSetsBeforeChange
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(ExerciseSetsBeforeChangeJson) ?? [];
        init => ExerciseSetsBeforeChangeJson = JsonSerializer.Serialize(value);
    }
}
