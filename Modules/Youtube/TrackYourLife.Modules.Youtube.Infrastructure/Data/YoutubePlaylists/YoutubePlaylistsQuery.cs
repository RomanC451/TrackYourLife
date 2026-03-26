using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubePlaylists;

internal sealed class YoutubePlaylistsQuery(YoutubeReadDbContext dbContext) : IYoutubePlaylistsQuery
{
    public async Task<IReadOnlyList<YoutubePlaylistReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .YoutubePlaylists.AsNoTracking()
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Name)
            .ThenBy(p => p.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<YoutubePlaylistReadModel?> GetByIdAndUserIdAsync(
        YoutubePlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .YoutubePlaylists.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId, cancellationToken);
    }

    public async Task<IReadOnlyList<YoutubePlaylistVideoReadModel>> GetVideosByPlaylistIdOrderedAsync(
        YoutubePlaylistId playlistId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .YoutubePlaylistVideos.AsNoTracking()
            .Where(v => v.YoutubePlaylistId == playlistId)
            .OrderBy(v => v.AddedOnUtc)
            .ThenBy(v => v.Id)
            .ToListAsync(cancellationToken);
    }
}
