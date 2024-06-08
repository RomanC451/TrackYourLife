using TrackYourLife.Modules.Users.Domain.UserGoals;

namespace TrackYourLife.Modules.Users.Contracts.UserGoals;

public sealed record UserGoalResponse(
    Guid Id,
    UserGoalType Type,
    int Value,
    UserGoalPerPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly? EndDate
);
