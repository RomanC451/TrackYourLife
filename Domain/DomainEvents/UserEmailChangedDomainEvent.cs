namespace TrackYourLifeDotnet.Domain.DomainEvents;

public sealed record UserEmailChangedDomainEvent(Guid Id, Guid UserId) : DomainEvent(Id);
