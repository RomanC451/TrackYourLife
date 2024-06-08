namespace TrackYourLife.Modules.Users.Domain.UserGoals;

public record UserGoalReadModel(
    Guid Id,
    Guid UserId,
    int Value,
    UserGoalType Type,
    UserGoalPerPeriod PerPeriod,
    DateOnly StartDate,
    DateOnly EndDate
);
