using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

public interface IGuitarPlaylistsRepository
{
    Task<GuitarPlaylist?> GetByIdAsync(
        GuitarPlaylistId id,
        CancellationToken cancellationToken = default
    );

    Task<GuitarPlaylist?> GetByIdAndUserIdAsync(
        GuitarPlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(GuitarPlaylist playlist, CancellationToken cancellationToken = default);

    void Remove(GuitarPlaylist playlist);

    void Update(GuitarPlaylist playlist);
}
