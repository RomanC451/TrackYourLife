﻿using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Users.Domain.Users.DomainEvents;

public sealed record UserEmailChangedDomainEvent(UserId UserId) : IDomainEvent { }
