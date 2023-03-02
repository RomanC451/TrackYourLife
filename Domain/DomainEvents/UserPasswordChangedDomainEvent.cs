namespace TrackYourLifeDotnet.Domain.DomainEvents;

public sealed record UserPasswordChangedDomainEvent(Guid Id, Guid UserId) : DomainEvent(Id);
