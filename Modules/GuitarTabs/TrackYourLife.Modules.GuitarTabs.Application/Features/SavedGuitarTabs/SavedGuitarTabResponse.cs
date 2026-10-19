namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;

public sealed record SavedGuitarTabResponse(
    Guid Id,
    string SongName,
    string ArtistName,
    string Type,
    string TabUrl
);
