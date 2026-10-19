namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;

public sealed record GuitarTabLibraryStateResponse(
    bool IsFavorite,
    IReadOnlyList<Guid> PlaylistIds,
    string? YoutubeUrl
);
