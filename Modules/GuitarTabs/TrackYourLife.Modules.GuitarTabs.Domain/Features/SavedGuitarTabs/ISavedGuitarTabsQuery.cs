using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

public interface ISavedGuitarTabsQuery
{
    Task<SavedGuitarTabReadModel?> GetByUserAndUrlAsync(
        UserId userId,
        string tabUrl,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<GuitarPlaylistId>> GetPlaylistIdsAsync(
        SavedGuitarTabId savedGuitarTabId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<SavedGuitarTabReadModel>> GetFavoritesAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyList<SavedGuitarTabReadModel>> GetByPlaylistAsync(
        UserId userId,
        GuitarPlaylistId playlistId,
        CancellationToken cancellationToken = default
    );
}
