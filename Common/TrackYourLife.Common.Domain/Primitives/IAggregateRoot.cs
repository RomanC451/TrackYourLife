namespace TrackYourLife.Common.Domain.Primitives;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
