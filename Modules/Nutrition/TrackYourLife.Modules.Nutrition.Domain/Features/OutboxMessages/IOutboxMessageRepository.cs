namespace TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages;

public interface INutritionOutboxMessageRepository
{
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken);
}
