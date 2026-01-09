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
        List<ExerciseSet>? newExerciseSets = null,
        List<ExerciseSet>? oldExerciseSets = null,
        bool? areChangesApplied = null,
        DateTime? createdOnUtc = null,
        DateTime? modifiedOnUtc = null
    )
    {
        var newExerciseSetsList = newExerciseSets ?? GenerateExerciseSets();
        var oldExerciseSetsList = oldExerciseSets ?? GenerateExerciseSets();

        return new ExerciseHistoryReadModel(
            id ?? ExerciseHistoryId.NewId(),
            ongoingTrainingId ?? OngoingTrainingId.NewId(),
            exerciseId ?? ExerciseId.NewId()
        )
        {
            NewExerciseSets = newExerciseSetsList,
            OldExerciseSets = oldExerciseSetsList,
            AreChangesApplied = areChangesApplied ?? f.Random.Bool(),
            CreatedOnUtc = createdOnUtc ?? f.Date.Recent().ToUniversalTime(),
            ModifiedOnUtc = modifiedOnUtc,
        };
    }

    public static ExerciseHistory GenerateEntity(
        ExerciseHistoryId? id = null,
        OngoingTrainingId? ongoingTrainingId = null,
        ExerciseId? exerciseId = null,
        List<ExerciseSet>? newExerciseSets = null,
        List<ExerciseSet>? oldExerciseSets = null,
        DateTime? createdOnUtc = null
    )
    {
        var newExerciseSetsList = newExerciseSets ?? GenerateExerciseSets();
        var oldExerciseSetsList = oldExerciseSets ?? GenerateExerciseSets();
        var createdOn = createdOnUtc ?? f.Date.Recent().ToUniversalTime();

        var result = ExerciseHistory.Create(
            id ?? ExerciseHistoryId.NewId(),
            ongoingTrainingId ?? OngoingTrainingId.NewId(),
            exerciseId ?? ExerciseId.NewId(),
            oldExerciseSetsList,
            newExerciseSetsList,
            ExerciseStatus.Completed,
            createdOn
        );

        return result.Value;
    }

    private static List<ExerciseSet> GenerateExerciseSets()
    {
        var setsCount = f.Random.Int(1, 5);
        var exerciseSets = new List<ExerciseSet>();

        for (int i = 0; i < setsCount; i++)
        {
            exerciseSets.Add(
                ExerciseSet.Create(
                    Guid.NewGuid(),
                    $"Set {i + 1}",
                    i,
                    f.Random.Float(5, 20),
                    "reps",
                    f.Random.Float(10, 100),
                    "kg"
                ).Value
            );
        }

        return exerciseSets;
    }
}
