namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.RemoveGuitarTabFromPlaylist;

internal sealed class RemoveGuitarTabFromPlaylistCommandHandler(IGuitarTabLibrary guitarTabLibrary)
    : ICommandHandler<RemoveGuitarTabFromPlaylistCommand>
{
    public Task<Result> Handle(
        RemoveGuitarTabFromPlaylistCommand request,
        CancellationToken cancellationToken
    ) =>
        guitarTabLibrary.RemoveFromPlaylistAsync(
            request.GuitarPlaylistId,
            request.SavedGuitarTabId,
            cancellationToken
        );
}
