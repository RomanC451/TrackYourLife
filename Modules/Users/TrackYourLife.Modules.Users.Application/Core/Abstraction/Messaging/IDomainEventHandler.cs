﻿using MediatR;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;

internal interface IDomainEventHandler<in TEvent> : INotificationHandler<TEvent>
    where TEvent : IDomainEvent { }
