using MassTransit;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Common.Infrastructure.Services;

internal sealed class IntegrationEventPublisher(IBus bus) : IIntegrationEventPublisher
{
    public Task PublishAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default
    )
        where TIntegrationEvent : class
    {
        return bus.Publish(integrationEvent, cancellationToken);
    }
}
