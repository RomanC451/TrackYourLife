using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

public interface IGuitarPlaylistsQuery
{
    Task<IReadOnlyList<GuitarPlaylistReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<GuitarPlaylistReadModel?> GetByIdAndUserIdAsync(
        GuitarPlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
