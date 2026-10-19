using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.UpdatePlaylist;

internal sealed class UpdatePlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarPlaylistsRepository guitarPlaylistsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdatePlaylistCommand>
{
    public async Task<Result> Handle(
        UpdatePlaylistCommand request,
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

        var updateResult = playlist.UpdateName(request.Name, dateTimeProvider.UtcNow);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        guitarPlaylistsRepository.Update(playlist);

        return Result.Success();
    }
}
