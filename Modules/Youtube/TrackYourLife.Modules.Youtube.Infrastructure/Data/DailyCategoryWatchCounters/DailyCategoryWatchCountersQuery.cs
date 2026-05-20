using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.DailyCategoryWatchCounters;

internal sealed class DailyCategoryWatchCountersQuery(YoutubeReadDbContext dbContext)
    : IDailyCategoryWatchCountersQuery
{
    public async Task<IReadOnlyList<DailyCategoryWatchCounterReadModel>> ListByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .DailyCategoryWatchCounters.AsNoTracking()
            .Where(c => c.UserId == userId && c.Date == date)
            .OrderBy(c => c.YoutubeCategoryId)
            .ToListAsync(cancellationToken);
    }
}
