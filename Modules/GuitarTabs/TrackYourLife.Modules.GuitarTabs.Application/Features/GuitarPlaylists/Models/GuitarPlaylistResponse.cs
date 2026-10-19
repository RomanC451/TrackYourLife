namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Models;

public sealed record GuitarPlaylistResponse(
    Guid Id,
    string Name,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
);
