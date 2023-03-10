using MediatR;

namespace TrackYourLifeDotnet.Domain.Primitives;

public interface IDomainEvent : INotification
{
    public Guid Id { get; init; }
}
