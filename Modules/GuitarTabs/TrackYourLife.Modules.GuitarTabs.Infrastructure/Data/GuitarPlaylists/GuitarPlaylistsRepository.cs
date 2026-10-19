using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarPlaylists;

internal sealed class GuitarPlaylistsRepository(GuitarTabsWriteDbContext dbContext)
    : GenericRepository<GuitarPlaylist, GuitarPlaylistId>(dbContext.GuitarPlaylists),
        IGuitarPlaylistsRepository
{
    public async Task<GuitarPlaylist?> GetByIdAndUserIdAsync(
        GuitarPlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.GuitarPlaylists.FirstOrDefaultAsync(
            playlist => playlist.Id == id && playlist.UserId == userId,
            cancellationToken
        );
    }
}
