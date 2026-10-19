using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Models;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Queries.GetPlaylistById;

public sealed record GetPlaylistByIdQuery(GuitarPlaylistId GuitarPlaylistId)
    : IQuery<GuitarPlaylistResponse>;
