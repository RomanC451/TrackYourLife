namespace TrackYourLife.Common.Domain.OutboxMessages;

public interface IOutboxMessageRepository
{
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken);
}
