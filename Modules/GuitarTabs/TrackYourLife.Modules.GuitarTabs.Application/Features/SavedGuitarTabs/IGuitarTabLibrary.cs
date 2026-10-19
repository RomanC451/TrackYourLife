using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;

public interface IGuitarTabLibrary
{
    Task<Result<GuitarTabLibraryStateResponse>> SetFavoriteAsync(
        GuitarTabSaveDetails details,
        bool isFavorite,
        CancellationToken cancellationToken
    );

    Task<Result<GuitarTabLibraryStateResponse>> SetPlaylistsAsync(
        GuitarTabSaveDetails details,
        IReadOnlyList<GuitarPlaylistId> playlistIds,
        CancellationToken cancellationToken
    );

    Task<Result> RemoveFromPlaylistAsync(
        GuitarPlaylistId playlistId,
        SavedGuitarTabId savedGuitarTabId,
        CancellationToken cancellationToken
    );

    Task RemoveSongsOnlyInPlaylistAsync(
        GuitarPlaylistId playlistId,
        CancellationToken cancellationToken
    );

    Task<Result<GuitarTabLibraryStateResponse>> SetYoutubeUrlAsync(
        GuitarTabSaveDetails details,
        string? youtubeUrl,
        CancellationToken cancellationToken
    );
}
