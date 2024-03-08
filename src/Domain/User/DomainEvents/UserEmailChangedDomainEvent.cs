using TrackYourLifeDotnet.Domain.Primitives;
using TrackYourLifeDotnet.Domain.Users.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Users.DomainEvents;

public sealed record UserEmailChangedDomainEvent(UserId UserId) : IDomainEvent { }
