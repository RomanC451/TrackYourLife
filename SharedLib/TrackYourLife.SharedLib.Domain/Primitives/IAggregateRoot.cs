namespace TrackYourLife.SharedLib.Domain.Primitives;

public interface IAggregateRoot
{
    IReadOnlyCollection<IOutboxDomainEvent> GetOutboxDomainEvents();
    void ClearOutboxDomainEvents();

    IReadOnlyCollection<IDirectDomainEvent> GetDirectDomainEvents();

    void ClearDirectDomainEvents();

    void OnDelete();
}
