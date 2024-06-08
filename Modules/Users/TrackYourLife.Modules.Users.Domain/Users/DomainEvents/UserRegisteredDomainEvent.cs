using TrackYourLife.Common.Domain.Primitives;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.Users.DomainEvents;

public sealed record UserRegisteredDomainEvent(UserId UserId) : IDomainEvent;
