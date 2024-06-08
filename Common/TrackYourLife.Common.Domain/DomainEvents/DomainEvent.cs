using TrackYourLife.Common.Domain.Primitives;

namespace TrackYourLife.Common.Domain.DomainEvents;

public abstract record DomainEvent<TId>(TId Id) : IDomainEvent;
