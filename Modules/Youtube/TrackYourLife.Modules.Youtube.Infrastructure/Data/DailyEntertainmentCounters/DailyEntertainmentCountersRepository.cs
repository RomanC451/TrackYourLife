using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.DailyEntertainmentCounters;

internal sealed class DailyEntertainmentCountersRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<DailyEntertainmentCounter, DailyEntertainmentCounterId>(
        dbContext.DailyEntertainmentCounters
    ),
        IDailyEntertainmentCountersRepository
{
    public async Task<DailyEntertainmentCounter?> GetByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .DailyEntertainmentCounters.Where(c => c.UserId == userId && c.Date == date)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
