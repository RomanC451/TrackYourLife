using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.WatchedVideos;

internal sealed class WatchedVideosRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<WatchedVideo, WatchedVideoId>(dbContext.WatchedVideos),
        IWatchedVideosRepository
{
    public async Task<WatchedVideo?> GetByUserIdAndVideoIdAsync(
        UserId userId,
        string videoId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .WatchedVideos.Where(w => w.UserId == userId && w.VideoId == videoId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
