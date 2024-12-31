using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.OutboxMessages;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Outbox;

internal sealed class OutboxMessageRepository(NutritionWriteDbContext dbContext)
    : INutritionOutboxMessageRepository
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
