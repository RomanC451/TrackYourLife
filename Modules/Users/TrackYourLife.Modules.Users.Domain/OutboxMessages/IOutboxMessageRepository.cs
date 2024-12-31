namespace TrackYourLife.Modules.Users.Domain.OutboxMessages;

public interface IUsersOutboxMessageRepository
{
    Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(CancellationToken cancellationToken);
}
