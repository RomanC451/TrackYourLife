using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Domain.Primitives;

public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : IStronglyTypedGuid
{
    private readonly List<IOutboxDomainEvent> _outboxDomainEvents = [];

    private readonly List<IDirectDomainEvent> _directDomainEvents = [];

    protected AggregateRoot(TId id)
        : base(id) { }

    protected AggregateRoot()
        : base() { }

    public IReadOnlyCollection<IOutboxDomainEvent> GetOutboxDomainEvents() =>
        _outboxDomainEvents.ToList();

    public void ClearOutboxDomainEvents() => _outboxDomainEvents.Clear();

    public IReadOnlyCollection<IDirectDomainEvent> GetDirectDomainEvents() =>
        _directDomainEvents.ToList();

    public void ClearDirectDomainEvents() => _directDomainEvents.Clear();

    protected void RaiseOutboxDomainEvent(IOutboxDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _outboxDomainEvents.Add(domainEvent);
    }

    protected void RaiseDirectDomainEvent(IDirectDomainEvent domainEvent)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        _directDomainEvents.Add(domainEvent);
    }

    public virtual void OnDelete()
    {
        // Base implementation
    }
}
