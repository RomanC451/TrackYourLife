using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Features.Goals.Events;

public sealed record GoalCreatedDomainEvent(UserId UserId, GoalId GoalId) : IOutboxDomainEvent;
