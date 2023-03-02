using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.DomainEvents;

public abstract record DomainEvent(Guid Id) : IDomainEvent;
