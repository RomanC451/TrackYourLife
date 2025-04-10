using Bogus;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Domain.UnitTests.Utils;

public static class GoalFaker
{
    private static readonly Faker f = new();

    public static Goal Generate(
        GoalId? id = null,
        UserId? userId = null,
        GoalType? type = null,
        int? value = null,
        GoalPeriod? period = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null
    )
    {
        return Goal.Create(
            id ?? GoalId.NewId(),
            userId ?? UserId.NewId(),
            type ?? f.PickRandom<GoalType>(),
            value ?? f.Random.Int(1, 100),
            period ?? f.PickRandom<GoalPeriod>(),
            startDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            endDate
        ).Value;
    }

    public static GoalReadModel GenerateReadModel(
        GoalId? id = null,
        UserId? userId = null,
        int? value = null,
        GoalType? type = null,
        GoalPeriod? period = null,
        DateOnly? startDate = null,
        DateOnly? endDate = null
    )
    {
        return new GoalReadModel(
            id ?? GoalId.NewId(),
            userId ?? UserId.NewId(),
            value ?? f.Random.Int(1, 100),
            type ?? f.PickRandom<GoalType>(),
            period ?? f.PickRandom<GoalPeriod>(),
            startDate ?? DateOnly.FromDateTime(DateTime.UtcNow),
            endDate ?? DateOnly.MaxValue
        );
    }
}
