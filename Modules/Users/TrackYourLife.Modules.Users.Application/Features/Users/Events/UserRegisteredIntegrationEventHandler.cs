using TrackYourLife.Modules.Users.Domain.Features.Users.DomainEvents;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Application.Abstraction.Messaging;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;

namespace TrackYourLife.Modules.Users.Application.Features.Users.Events;

internal sealed class UserRegisteredIntegrationEventHandler(
    IIntegrationEventPublisher integrationEventPublisher
) : IIntegrationEventHandler<UserRegisteredDomainEvent>
{
    public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        return integrationEventPublisher.PublishAsync(
            new UserRegisteredIntegrationEvent(notification.UserId),
            cancellationToken
        );
    }
}
