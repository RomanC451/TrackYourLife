using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Contracts.UserGoals;

public sealed record UpdateUserGoalRequest
(
    UserGoalId Id,
    UserGoalType Type,
    int Value,
    UserGoalPerPeriod PerPeriod,
    DateOnly StartDate,
    bool? Force,
    DateOnly? EndDate
);
