﻿using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Features.Users.DomainEvents;

public sealed record UserPasswordChangedDomainEvent(UserId Id) : IOutboxDomainEvent;
