using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.DomainEvents;

public sealed record UserRegisteredDomainEvent(Guid UserId) : IDomainEvent
{
}
