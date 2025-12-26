using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class ExerciseReadModelFaker
{
    private static readonly Faker f = new();

    public static ExerciseReadModel Generate(
        ExerciseId? id = null,
        UserId? userId = null,
        string? name = null,
        List<string>? muscleGroups = null,
        Difficulty? difficulty = null,
        string? pictureUrl = null,
        string? videoUrl = null,
        string? description = null,
        string? equipment = null,
        DateTime? createdOnUtc = null,
        DateTime? modifiedOnUtc = null,
        List<ExerciseSet>? exerciseSets = null
    )
    {
        var exerciseSetsList = exerciseSets ?? GenerateExerciseSets();

        return new ExerciseReadModel(
            id ?? ExerciseId.NewId(),
            userId ?? UserId.NewId(),
            name ?? f.Random.Words(2),
            muscleGroups
                ?? f.PickRandom(
                        new[] { "Chest", "Triceps", "Shoulders", "Back", "Legs" },
                        f.Random.Int(1, 3)
                    )
                    .ToList(),
            difficulty ?? f.PickRandom<Difficulty>(),
            pictureUrl,
            videoUrl ?? f.Internet.Url(),
            description ?? f.Lorem.Sentence(),
            equipment ?? f.PickRandom("Barbell", "Dumbbell", "Machine", "Bodyweight", "Kettlebell"),
            createdOnUtc ?? f.Date.Recent(),
            modifiedOnUtc
        )
        {
            ExerciseSets = exerciseSetsList,
        };
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
