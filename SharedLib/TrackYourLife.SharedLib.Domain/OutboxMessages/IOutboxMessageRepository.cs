namespace TrackYourLife.SharedLib.Domain.OutboxMessages;

public interface IOutboxMessageRepository
{
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken);
}
