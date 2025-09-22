using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Exercises;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class ExerciseFaker
{
    private static readonly Faker f = new();

    public static Exercise Generate(
        ExerciseId? id = null,
        UserId? userId = null,
        string? name = null,
        List<string>? muscleGroups = null,
        Difficulty? difficulty = null,
        string? pictureUrl = null,
        string? videoUrl = null,
        string? description = null,
        string? equipment = null,
        List<ExerciseSet>? exerciseSets = null,
        DateTime? createdOnUtc = null
    )
    {
        var exerciseSetsList = exerciseSets ?? GenerateExerciseSets();
        
        var result = Exercise.Create(
            id ?? ExerciseId.NewId(),
            userId ?? UserId.NewId(),
            name ?? f.Random.Words(2),
            muscleGroups ?? f.PickRandom(new[] { "Chest", "Triceps", "Shoulders", "Back", "Legs" }, f.Random.Int(1, 3)).ToList(),
            difficulty ?? f.PickRandom<Difficulty>(),
            pictureUrl ?? f.Internet.Url(),
            videoUrl ?? f.Internet.Url(),
            description ?? f.Lorem.Sentence(),
            equipment ?? f.PickRandom("Barbell", "Dumbbell", "Machine", "Bodyweight", "Kettlebell"),
            exerciseSetsList,
            createdOnUtc ?? f.Date.Recent()
        );

        return result.Value;
    }

    private static List<ExerciseSet> GenerateExerciseSets()
    {
        var setsCount = f.Random.Int(1, 5);
        var exerciseSets = new List<ExerciseSet>();
        
        for (int i = 0; i < setsCount; i++)
        {
            exerciseSets.Add(new ExerciseSet(
                Guid.NewGuid(),
                $"Set {i + 1}",
                f.Random.Int(5, 20),
                f.Random.Float(10, 100),
                i
            ));
        }
        
        return exerciseSets;
    }
}
