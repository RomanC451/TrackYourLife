
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Goals;

public sealed record GoalReadModel(
    GoalId Id,
    UserId UserId,
    int Value,
    GoalType Type,
    GoalPeriod Period,
    DateOnly StartDate,
    DateOnly EndDate
) : IReadModel<GoalId>;
