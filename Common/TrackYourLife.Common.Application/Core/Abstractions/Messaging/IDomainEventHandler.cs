using MediatR;
using TrackYourLife.Common.Domain.Primitives;

namespace TrackYourLife.Common.Application.Core.Abstractions.Messaging;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent
{ }
