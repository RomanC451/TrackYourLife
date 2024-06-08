using Microsoft.EntityFrameworkCore;
using TrackYourLife.Common.Domain.OutboxMessages;

namespace TrackYourLife.Common.Persistence.Repositories;

internal sealed class OutboxMessageRepository(ApplicationWriteDbContext dbContext) : IOutboxMessageRepository
{
    private readonly ApplicationWriteDbContext _dbContext = dbContext;

    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(
        CancellationToken cancellationToken
    )
    {
        return await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}
