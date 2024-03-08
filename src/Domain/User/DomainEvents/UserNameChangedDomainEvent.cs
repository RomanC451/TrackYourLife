using TrackYourLifeDotnet.Domain.DomainEvents;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Users.DomainEvents;

public sealed record UserNameChangedDomainEvent(UserId Id) : DomainEvent<UserId>(Id);
