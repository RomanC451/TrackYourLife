using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

public sealed record GuitarPlaylistReadModel(
    GuitarPlaylistId Id,
    UserId UserId,
    string Name,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<GuitarPlaylistId>;
