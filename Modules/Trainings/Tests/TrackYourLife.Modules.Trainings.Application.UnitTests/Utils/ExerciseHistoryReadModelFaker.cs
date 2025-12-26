using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class ExerciseHistoryReadModelFaker
{
    private static readonly Faker f = new();

    public static ExerciseHistoryReadModel Generate(
        ExerciseHistoryId? id = null,
        OngoingTrainingId? ongoingTrainingId = null,
        ExerciseId? exerciseId = null,
        List<ExerciseSetChange>? exerciseSetChanges = null,
        List<ExerciseSet>? exerciseSetsBeforeChange = null,
        bool? areChangesApplied = null,
        DateTime? createdOnUtc = null,
        DateTime? modifiedOnUtc = null
    )
    {
        var exerciseSetChangesList = exerciseSetChanges ?? GenerateExerciseSetChanges();
        var exerciseSetsBeforeChangeList = exerciseSetsBeforeChange ?? GenerateExerciseSets();

        return new ExerciseHistoryReadModel(
            id ?? ExerciseHistoryId.NewId(),
            ongoingTrainingId ?? OngoingTrainingId.NewId(),
            exerciseId ?? ExerciseId.NewId()
        )
        {
            ExerciseSetChanges = exerciseSetChangesList,
            ExerciseSetsBeforeChange = exerciseSetsBeforeChangeList,
            AreChangesApplied = areChangesApplied ?? f.Random.Bool(),
            CreatedOnUtc = createdOnUtc ?? f.Date.Recent(),
            ModifiedOnUtc = modifiedOnUtc,
        };
    }

    private static List<ExerciseSetChange> GenerateExerciseSetChanges()
    {
        var changesCount = f.Random.Int(0, 3);
        var changes = new List<ExerciseSetChange>();

        for (int i = 0; i < changesCount; i++)
        {
            changes.Add(
                new WeightBasedExerciseSetChange(
                    Guid.NewGuid(),
                    f.Random.Float(-10, 10),
                    f.Random.Int(-5, 5)
                )
            );
        }

        return changes;
    }

    private static List<ExerciseSet> GenerateExerciseSets()
    {
        var setsCount = f.Random.Int(1, 5);
        var exerciseSets = new List<ExerciseSet>();

        for (int i = 0; i < setsCount; i++)
        {
            exerciseSets.Add(
                new WeightBasedExerciseSet(
                    Guid.NewGuid(),
                    $"Set {i + 1}",
                    i,
                    f.Random.Int(5, 20),
                    f.Random.Float(10, 100)
                )
            );
        }

        return exerciseSets;
    }
}
