using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.Modules.Trainings.Domain.Features.ExercisesHistories;
using TrackYourLife.Modules.Trainings.Domain.Features.OngoingTrainings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class ExerciseHistoryFaker
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

    public static ExerciseHistory GenerateEntity(
        ExerciseHistoryId? id = null,
        OngoingTrainingId? ongoingTrainingId = null,
        ExerciseId? exerciseId = null,
        List<ExerciseSetChange>? exerciseSetChanges = null,
        List<ExerciseSet>? exerciseSetsBeforeChange = null,
        DateTime? createdOnUtc = null
    )
    {
        var exerciseSetChangesList = exerciseSetChanges ?? GenerateExerciseSetChanges();
        var exerciseSetsBeforeChangeList = exerciseSetsBeforeChange ?? GenerateExerciseSets();
        var createdOn = createdOnUtc ?? f.Date.Recent();

        var result = ExerciseHistory.Create(
            id ?? ExerciseHistoryId.NewId(),
            ongoingTrainingId ?? OngoingTrainingId.NewId(),
            exerciseId ?? ExerciseId.NewId(),
            exerciseSetsBeforeChangeList,
            exerciseSetChangesList,
            createdOn
        );

        return result.Value;
    }

    private static List<ExerciseSetChange> GenerateExerciseSetChanges()
    {
        return f.Make(
                f.Random.Int(1, 5),
                () =>
                    new ExerciseSetChange
                    {
                        SetId = f.Random.Guid(),
                        WeightChange = f.Random.Float(0, 50),
                        RepsChange = f.Random.Int(-5, 5),
                    }
            )
            .ToList();
    }

    private static List<ExerciseSet> GenerateExerciseSets()
    {
        return f.Make(
                f.Random.Int(1, 5),
                () =>
                    new ExerciseSet(
                        f.Random.Guid(),
                        f.Random.String(10),
                        f.Random.Int(1, 20),
                        f.Random.Float(10, 200),
                        f.Random.Int(1, 10)
                    )
            )
            .ToList();
    }
}
