using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;

public sealed record GuitarPlaylistSongReadModel(
    GuitarPlaylistSongId Id,
    GuitarPlaylistId GuitarPlaylistId,
    SavedGuitarTabId SavedGuitarTabId,
    DateTime AddedOnUtc
) : IReadModel<GuitarPlaylistSongId>;
