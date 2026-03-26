using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.DeletePlaylist;

internal sealed class DeletePlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubePlaylistsRepository youtubePlaylistsRepository
) : ICommandHandler<DeletePlaylistCommand>
{
    public async Task<Result> Handle(
        DeletePlaylistCommand request,
        CancellationToken cancellationToken
    )
    {
        var playlist = await youtubePlaylistsRepository.GetByIdAndUserIdAsync(
            request.YoutubePlaylistId,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (playlist is null)
        {
            return Result.Failure(YoutubePlaylistErrors.NotFound(request.YoutubePlaylistId.Value));
        }

        youtubePlaylistsRepository.Remove(playlist);

        return Result.Success();
    }
}
