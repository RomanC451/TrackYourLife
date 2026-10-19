namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabYoutube;

internal sealed class SetGuitarTabYoutubeCommandHandler(IGuitarTabLibrary guitarTabLibrary)
    : ICommandHandler<SetGuitarTabYoutubeCommand, GuitarTabLibraryStateResponse>
{
    public Task<Result<GuitarTabLibraryStateResponse>> Handle(
        SetGuitarTabYoutubeCommand request,
        CancellationToken cancellationToken
    ) =>
        guitarTabLibrary.SetYoutubeUrlAsync(
            new GuitarTabSaveDetails(request.TabUrl, request.SongName, request.ArtistName, request.Type),
            request.YoutubeUrl,
            cancellationToken
        );
}
