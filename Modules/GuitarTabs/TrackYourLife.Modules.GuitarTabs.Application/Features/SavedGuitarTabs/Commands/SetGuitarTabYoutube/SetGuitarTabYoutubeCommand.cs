namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabYoutube;

public sealed record SetGuitarTabYoutubeCommand(
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type,
    string? YoutubeUrl
) : ICommand<GuitarTabLibraryStateResponse>;
