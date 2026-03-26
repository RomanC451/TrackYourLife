using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Data.YoutubePlaylists;

internal sealed class YoutubePlaylistsRepository(YoutubeWriteDbContext dbContext)
    : GenericRepository<YoutubePlaylist, YoutubePlaylistId>(dbContext.YoutubePlaylists),
        IYoutubePlaylistsRepository
{
    public async Task<YoutubePlaylist?> GetByIdAndUserIdAsync(
        YoutubePlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.YoutubePlaylists.FirstOrDefaultAsync(
            p => p.Id == id && p.UserId == userId,
            cancellationToken
        );
    }
}
