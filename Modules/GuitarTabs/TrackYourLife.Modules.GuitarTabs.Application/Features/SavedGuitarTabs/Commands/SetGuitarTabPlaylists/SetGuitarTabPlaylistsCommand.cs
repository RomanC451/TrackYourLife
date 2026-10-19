namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabPlaylists;

public sealed record SetGuitarTabPlaylistsCommand(
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type,
    IReadOnlyList<Guid> PlaylistIds
) : ICommand<GuitarTabLibraryStateResponse>;
