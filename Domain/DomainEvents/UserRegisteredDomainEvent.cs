namespace TrackYourLifeDotnet.Domain.DomainEvents;

public sealed record UserRegisteredDomainEvent(Guid Id, Guid UserId) : DomainEvent(Id);
