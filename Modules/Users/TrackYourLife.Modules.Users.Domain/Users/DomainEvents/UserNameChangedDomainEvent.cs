using TrackYourLife.Common.Domain.DomainEvents;
using TrackYourLife.Modules.Users.Domain.Users.StrongTypes;

namespace TrackYourLife.Modules.Users.Domain.Users.DomainEvents;

public sealed record UserNameChangedDomainEvent(UserId Id) : DomainEvent<UserId>(Id);
