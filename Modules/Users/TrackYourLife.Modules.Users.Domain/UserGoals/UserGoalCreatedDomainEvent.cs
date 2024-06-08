using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.UserGoals;

public sealed record UserGoalCreatedDomainEvent(
    UserId UserId,
    UserGoalId UserGoalId,
    UserGoalType Type,
    DateOnly StartDate
) : IDomainEvent;
