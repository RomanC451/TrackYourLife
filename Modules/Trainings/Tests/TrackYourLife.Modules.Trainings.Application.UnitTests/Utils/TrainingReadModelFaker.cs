using Bogus;
using TrackYourLife.Modules.Trainings.Domain.Core;
using TrackYourLife.Modules.Trainings.Domain.Features.Trainings;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Trainings.Application.UnitTests.Utils;

public static class TrainingReadModelFaker
{
    private static readonly Faker f = new();

    public static TrainingReadModel Generate(
        TrainingId? id = null,
        UserId? userId = null,
        string? name = null,
        List<string>? muscleGroups = null,
        Difficulty? difficulty = null,
        string? description = null,
        DateTime? createdOnUtc = null,
        int? duration = null,
        int? restSeconds = null,
        DateTime? modifiedOnUtc = null
    )
    {
        return new TrainingReadModel(
            id ?? TrainingId.NewId(),
            userId ?? UserId.NewId(),
            name ?? f.Random.Words(2),
            muscleGroups ?? f.PickRandom(new[] { "Chest", "Triceps", "Shoulders", "Back", "Legs" }, f.Random.Int(1, 3)).ToList(),
            difficulty ?? f.PickRandom<Difficulty>(),
            description ?? f.Lorem.Sentence(),
            createdOnUtc ?? f.Date.Recent(),
            duration ?? f.Random.Int(15, 120),
            restSeconds ?? f.Random.Int(30, 180),
            modifiedOnUtc
        );
    }
}
