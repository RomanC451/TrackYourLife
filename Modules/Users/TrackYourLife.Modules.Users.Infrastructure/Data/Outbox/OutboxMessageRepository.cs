using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Users.Domain.OutboxMessages;

namespace TrackYourLife.Modules.Users.Infrastructure.Data.Outbox;

internal sealed class OutboxMessageRepository(UsersWriteDbContext dbContext)
    : IUsersOutboxMessageRepository
{
    public async Task<List<OutboxMessage>> GetUnprocessedMessagesAsync(
        CancellationToken cancellationToken
    )
    {
        return await dbContext
            .OutboxMessages.Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20)
            .ToListAsync(cancellationToken);
    }
}
