using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Features.Goals;

public sealed record GoalReadModel(
    GoalId Id,
    UserId UserId,
    int Value,
    GoalType Type,
    GoalPeriod Period,
    DateOnly StartDate,
    DateOnly EndDate
) : IReadModel<GoalId>;
