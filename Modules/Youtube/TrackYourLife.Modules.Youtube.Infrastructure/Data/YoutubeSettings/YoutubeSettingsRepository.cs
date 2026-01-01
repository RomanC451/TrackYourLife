using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeSettings;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubeSettings;

internal sealed class YoutubeSettingsRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<YoutubeSetting, YoutubeSettingsId>(dbContext.YoutubeSettings),
        IYoutubeSettingsRepository
{
    public async Task<YoutubeSetting?> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .YoutubeSettings.Where(s => s.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
