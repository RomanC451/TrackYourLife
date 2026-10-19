using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.DeletePlaylist;

internal sealed class DeletePlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarPlaylistsRepository guitarPlaylistsRepository,
    IGuitarTabLibrary guitarTabLibrary
) : ICommandHandler<DeletePlaylistCommand>
{
    public async Task<Result> Handle(
        DeletePlaylistCommand request,
        CancellationToken cancellationToken
    )
    {
        var playlist = await guitarPlaylistsRepository.GetByIdAndUserIdAsync(
            request.GuitarPlaylistId,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (playlist is null)
        {
            return Result.Failure(GuitarPlaylistErrors.NotFound(request.GuitarPlaylistId.Value));
        }

        await guitarTabLibrary.RemoveSongsOnlyInPlaylistAsync(
            request.GuitarPlaylistId,
            cancellationToken
        );
        guitarPlaylistsRepository.Remove(playlist);

        return Result.Success();
    }
}
