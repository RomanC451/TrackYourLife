namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabFavorite;

public sealed record SetGuitarTabFavoriteCommand(
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type,
    bool IsFavorite
) : ICommand<GuitarTabLibraryStateResponse>;
