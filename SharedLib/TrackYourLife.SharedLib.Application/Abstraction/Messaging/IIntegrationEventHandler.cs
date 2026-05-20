using MediatR;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.SharedLib.Application.Abstraction.Messaging;

public interface IIntegrationEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent { }
