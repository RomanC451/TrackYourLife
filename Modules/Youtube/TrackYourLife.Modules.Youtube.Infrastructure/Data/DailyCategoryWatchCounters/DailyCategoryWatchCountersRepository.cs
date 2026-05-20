using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.DailyCategoryWatchCounters;

internal sealed class DailyCategoryWatchCountersRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<DailyCategoryWatchCounter, DailyCategoryWatchCounterId>(
        dbContext.DailyCategoryWatchCounters
    ),
        IDailyCategoryWatchCountersRepository
{
    public async Task<DailyCategoryWatchCounter?> GetByUserIdDateAndCategoryAsync(
        UserId userId,
        DateOnly date,
        YoutubeCategoryId categoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.DailyCategoryWatchCounters.FirstOrDefaultAsync(
            c => c.UserId == userId && c.Date == date && c.YoutubeCategoryId == categoryId,
            cancellationToken
        );
    }
}
