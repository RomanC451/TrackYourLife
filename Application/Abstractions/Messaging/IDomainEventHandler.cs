using TrackYourLifeDotnet.Domain.Primitives;
using MediatR;

namespace TrackYourLifeDotnet.Application.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent { }
