namespace TrackYourLife.SharedLib.Application.Abstraction;

public interface IIntegrationEventPublisher
{
    Task PublishAsync<TIntegrationEvent>(
        TIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default
    )
        where TIntegrationEvent : class;
}
