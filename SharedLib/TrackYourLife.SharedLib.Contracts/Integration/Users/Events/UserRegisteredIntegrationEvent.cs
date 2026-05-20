using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.SharedLib.Contracts.Integration.Users.Events;

public sealed record UserRegisteredIntegrationEvent(UserId UserId);
