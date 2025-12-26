using System.Text.Json;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;

public sealed record ExerciseHistoryReadModel(
    ExerciseHistoryId Id,
    OngoingTrainingId OngoingTrainingId,
    ExerciseId ExerciseId,
    string NewExerciseSetsJson = "[]",
    string OldExerciseSetsJson = "[]",
    bool AreChangesApplied = false,
    DateTime CreatedOnUtc = default,
    DateTime? ModifiedOnUtc = null
) : IReadModel<ExerciseHistoryId>
{
    public IReadOnlyList<ExerciseSet> NewExerciseSets
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(NewExerciseSetsJson) ?? new();
        init => NewExerciseSetsJson = JsonSerializer.Serialize(value.ToList());
    }

    public IReadOnlyList<ExerciseSet> OldExerciseSets
    {
        get => JsonSerializer.Deserialize<List<ExerciseSet>>(OldExerciseSetsJson) ?? new();
        init => OldExerciseSetsJson = JsonSerializer.Serialize(value.ToList());
    }
}
