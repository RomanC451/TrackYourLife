using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubePlaylists;

internal sealed class YoutubePlaylistVideosRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<YoutubePlaylistVideo, YoutubePlaylistVideoId>(dbContext.YoutubePlaylistVideos),
        IYoutubePlaylistVideosRepository
{
    public async Task<YoutubePlaylistVideo?> GetByPlaylistIdAndYoutubeIdAsync(
        YoutubePlaylistId playlistId,
        string youtubeId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.YoutubePlaylistVideos.FirstOrDefaultAsync(
            v => v.YoutubePlaylistId == playlistId && v.YoutubeId == youtubeId,
            cancellationToken
        );
    }

    public async Task<bool> ExistsAsync(
        YoutubePlaylistId playlistId,
        string youtubeId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.YoutubePlaylistVideos.AnyAsync(
            v => v.YoutubePlaylistId == playlistId && v.YoutubeId == youtubeId,
            cancellationToken
        );
    }
}
