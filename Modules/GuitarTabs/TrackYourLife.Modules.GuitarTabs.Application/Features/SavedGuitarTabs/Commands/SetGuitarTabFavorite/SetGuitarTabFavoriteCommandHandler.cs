namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabFavorite;

internal sealed class SetGuitarTabFavoriteCommandHandler(IGuitarTabLibrary guitarTabLibrary)
    : ICommandHandler<SetGuitarTabFavoriteCommand, GuitarTabLibraryStateResponse>
{
    public Task<Result<GuitarTabLibraryStateResponse>> Handle(
        SetGuitarTabFavoriteCommand request,
        CancellationToken cancellationToken
    ) =>
        guitarTabLibrary.SetFavoriteAsync(
            new GuitarTabSaveDetails(request.TabUrl, request.SongName, request.ArtistName, request.Type),
            request.IsFavorite,
            cancellationToken
        );
}
