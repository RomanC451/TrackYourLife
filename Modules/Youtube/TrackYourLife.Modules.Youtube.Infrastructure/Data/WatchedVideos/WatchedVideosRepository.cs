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

    public async Task<IEnumerable<WatchedVideo>> GetByUserIdAndVideoIdsAsync(
        UserId userId,
        IEnumerable<string> videoIds,
        CancellationToken cancellationToken = default
    )
    {
        var videoIdsList = videoIds.ToList();
        return await dbContext
            .WatchedVideos.Where(w => w.UserId == userId && videoIdsList.Contains(w.VideoId))
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<WatchedVideo> Items, int TotalCount)> GetPagedByUserIdAsync(
        UserId userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var query = dbContext.WatchedVideos.Where(w => w.UserId == userId);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(w => w.WatchedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
