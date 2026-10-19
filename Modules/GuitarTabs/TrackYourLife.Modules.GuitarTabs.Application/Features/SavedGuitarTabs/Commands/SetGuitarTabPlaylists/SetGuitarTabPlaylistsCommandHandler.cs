using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabPlaylists;

internal sealed class SetGuitarTabPlaylistsCommandHandler(IGuitarTabLibrary guitarTabLibrary)
    : ICommandHandler<SetGuitarTabPlaylistsCommand, GuitarTabLibraryStateResponse>
{
    public Task<Result<GuitarTabLibraryStateResponse>> Handle(
        SetGuitarTabPlaylistsCommand request,
        CancellationToken cancellationToken
    ) =>
        guitarTabLibrary.SetPlaylistsAsync(
            new GuitarTabSaveDetails(request.TabUrl, request.SongName, request.ArtistName, request.Type),
            request.PlaylistIds.Select(GuitarPlaylistId.Create).ToArray(),
            cancellationToken
        );
}
