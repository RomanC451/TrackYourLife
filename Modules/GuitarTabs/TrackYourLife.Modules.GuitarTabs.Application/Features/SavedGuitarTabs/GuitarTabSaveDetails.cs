namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;

public sealed record GuitarTabSaveDetails(
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type
);
