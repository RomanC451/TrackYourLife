using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Contracts.UserGoals;

public sealed record AddUserGoalRequest
(
    int Value,
    UserGoalType Type,
    UserGoalPerPeriod PerPeriod,
    DateOnly StartDate,
    bool? Force,
    DateOnly? EndDate
);
