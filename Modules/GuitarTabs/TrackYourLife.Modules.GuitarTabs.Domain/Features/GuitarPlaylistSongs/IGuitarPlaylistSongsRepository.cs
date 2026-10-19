using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;

public interface IGuitarPlaylistSongsRepository
{
    Task<IReadOnlyList<GuitarPlaylistSong>> ListByTabIdAsync(
        SavedGuitarTabId savedGuitarTabId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<GuitarPlaylistSong>> ListByTabIdsAsync(
        IReadOnlyCollection<SavedGuitarTabId> savedGuitarTabIds,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<GuitarPlaylistSong>> ListByPlaylistIdAsync(
        GuitarPlaylistId playlistId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(GuitarPlaylistSong song, CancellationToken cancellationToken = default);

    void Remove(GuitarPlaylistSong song);
}
