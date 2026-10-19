using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarPlaylistSongs;

internal sealed class GuitarPlaylistSongsRepository(GuitarTabsWriteDbContext dbContext)
    : GenericRepository<GuitarPlaylistSong, GuitarPlaylistSongId>(dbContext.GuitarPlaylistSongs),
        IGuitarPlaylistSongsRepository
{
    public async Task<IReadOnlyList<GuitarPlaylistSong>> ListByTabIdAsync(
        SavedGuitarTabId savedGuitarTabId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .GuitarPlaylistSongs.Where(song => song.SavedGuitarTabId == savedGuitarTabId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GuitarPlaylistSong>> ListByTabIdsAsync(
        IReadOnlyCollection<SavedGuitarTabId> savedGuitarTabIds,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .GuitarPlaylistSongs.Where(song => savedGuitarTabIds.Contains(song.SavedGuitarTabId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GuitarPlaylistSong>> ListByPlaylistIdAsync(
        GuitarPlaylistId playlistId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .GuitarPlaylistSongs.Where(song => song.GuitarPlaylistId == playlistId)
            .ToListAsync(cancellationToken);
    }
}
