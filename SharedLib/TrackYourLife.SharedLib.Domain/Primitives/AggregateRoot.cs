using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Domain.Primitives;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : IStronglyTypedGuid
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected AggregateRoot(TId id)
        : base(id) { }

    protected AggregateRoot()
        : base() { }

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents.ToList();

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
