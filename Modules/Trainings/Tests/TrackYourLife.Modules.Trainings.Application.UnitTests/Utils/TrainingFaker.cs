using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Features.TrainingExercises;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class TrainingFaker
{
    private static readonly Faker f = new();

    public static Training Generate(
        TrainingId? id = null,
        UserId? userId = null,
        string? name = null,
        List<string>? muscleGroups = null,
        Difficulty? difficulty = null,
        string? description = null,
        DateTime? createdOnUtc = null,
        int? duration = null,
        int? restSeconds = null,
        List<Exercise>? exercises = null
    )
    {
        var trainingExercises =
            exercises
                ?.Select(
                    (exercise, index) =>
                        TrainingExercise.Create(id ?? TrainingId.NewId(), exercise, index).Value
                )
                .ToList() ?? GenerateTrainingExercises(id ?? TrainingId.NewId());

        var result = Training.Create(
            id ?? TrainingId.NewId(),
            userId ?? UserId.NewId(),
            name ?? f.Random.Words(2),
            muscleGroups
                ?? f.PickRandom(
                        new[] { "Chest", "Triceps", "Shoulders", "Back", "Legs" },
                        f.Random.Int(1, 3)
                    )
                    .ToList(),
            difficulty ?? f.PickRandom<Difficulty>(),
            trainingExercises,
            createdOnUtc ?? f.Date.Recent().ToUniversalTime(),
            duration ?? f.Random.Int(15, 120),
            restSeconds ?? f.Random.Int(30, 180),
            description ?? f.Lorem.Sentence()
        );

        return result.Value;
    }

    private static List<TrainingExercise> GenerateTrainingExercises(TrainingId trainingId)
    {
        var faker = new Faker();
        var exercises = new List<Exercise>();

        // Generate 2-4 exercises
        for (int i = 0; i < faker.Random.Int(2, 4); i++)
        {
            var exercise = ExerciseFaker.Generate();
            exercises.Add(exercise);
        }

        return exercises
            .Select((exercise, index) => TrainingExercise.Create(trainingId, exercise, index).Value)
            .ToList();
    }
}
