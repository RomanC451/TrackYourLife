using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.DomainEvents;

public abstract record DomainEvent<TId>(TId Id) : IDomainEvent;
