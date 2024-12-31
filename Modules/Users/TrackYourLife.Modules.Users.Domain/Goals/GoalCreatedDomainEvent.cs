using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Goals;

public sealed record GoalCreatedDomainEvent(
    UserId UserId,
    GoalId UserGoalId,
    GoalType Type,
    DateOnly StartDate
) : IDomainEvent;
