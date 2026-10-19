using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetPlaylistSongs;

public sealed record GetPlaylistSongsQuery(GuitarPlaylistId GuitarPlaylistId)
    : IQuery<IReadOnlyList<SavedGuitarTabResponse>>;
