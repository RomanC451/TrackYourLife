using MediatR;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Common.Application.Core.Abstractions.Messaging;

public interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent;
