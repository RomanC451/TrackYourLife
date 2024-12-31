using TrackYourLife.Modules.Users.Domain.Goals;

namespace TrackYourLife.Modules.Users.Contracts.Goals;

public sealed record GoalDto(
    GoalId Id,
    GoalType Type,
    int Value,
    GoalPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly EndDate
);
