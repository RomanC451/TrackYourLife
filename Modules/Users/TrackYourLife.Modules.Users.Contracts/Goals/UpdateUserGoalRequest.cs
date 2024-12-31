using TrackYourLife.Modules.Users.Domain.Goals;

namespace TrackYourLife.Modules.Users.Contracts.Goals;

public sealed record UpdateUserGoalRequest(
    GoalId Id,
    GoalType Type,
    int Value,
    GoalPeriod PerPeriod,
    DateOnly StartDate,
    bool? Force,
    DateOnly? EndDate
);
