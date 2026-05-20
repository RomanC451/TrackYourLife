using TrackYourLife.Modules.Users.Application.Features.Users.Events;
using TrackYourLife.Modules.Users.Domain.Features.Users.DomainEvents;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Integration.Users.Events;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Users.Events;

public sealed class UserRegisteredIntegrationEventHandlerTests
{
    private readonly IIntegrationEventPublisher _integrationEventPublisher;
    private readonly UserRegisteredIntegrationEventHandler _handler;

    public UserRegisteredIntegrationEventHandlerTests()
    {
        _integrationEventPublisher = Substitute.For<IIntegrationEventPublisher>();
        _handler = new UserRegisteredIntegrationEventHandler(_integrationEventPublisher);
    }

    [Fact]
    public async Task Handle_ShouldPublishUserRegisteredIntegrationEvent()
    {
        var userId = UserId.NewId();
        var @event = new UserRegisteredDomainEvent(userId);

        await _handler.Handle(@event, CancellationToken.None);

        await _integrationEventPublisher
            .Received(1)
            .PublishAsync(
                Arg.Is<UserRegisteredIntegrationEvent>(e => e.UserId == userId),
                Arg.Any<CancellationToken>()
            );
    }
}
