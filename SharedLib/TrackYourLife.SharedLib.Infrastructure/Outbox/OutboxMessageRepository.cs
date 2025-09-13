using Microsoft.EntityFrameworkCore;
using TrackYourLife.SharedLib.Domain.OutboxMessages;

namespace TrackYourLife.SharedLib.Infrastructure.Outbox;

public abstract class OutboxMessageRepository(DbSet<OutboxMessage> outboxMessages)
    : IOutboxMessageRepository
{
    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(
        CancellationToken cancellationToken
    )
    {
        return await outboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .Where(m => m.RetryCount < OutboxMessage.MaxRetryCount)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(OutboxMessage outboxMessage, CancellationToken cancellationToken)
    {
        await outboxMessages.AddAsync(outboxMessage, cancellationToken);
    }
}
