using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Models;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Queries.GetPlaylists;

public sealed record GetPlaylistsQuery : IQuery<IReadOnlyList<GuitarPlaylistResponse>>;
