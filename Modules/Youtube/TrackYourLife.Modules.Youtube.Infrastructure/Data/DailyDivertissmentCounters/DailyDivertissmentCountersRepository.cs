using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyDivertissmentCounters;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.DailyDivertissmentCounters;

internal sealed class DailyDivertissmentCountersRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<DailyDivertissmentCounter, DailyDivertissmentCounterId>(
        dbContext.DailyDivertissmentCounters
    ),
        IDailyDivertissmentCountersRepository
{
    public async Task<DailyDivertissmentCounter?> GetByUserIdAndDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .DailyDivertissmentCounters.Where(c => c.UserId == userId && c.Date == date)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
