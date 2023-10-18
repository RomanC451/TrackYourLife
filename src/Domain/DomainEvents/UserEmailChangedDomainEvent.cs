using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.DomainEvents;

public sealed record UserEmailChangedDomainEvent( Guid UserId ) : IDomainEvent
{
}
