using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.RemoveVideoFromPlaylist;

internal sealed class RemoveVideoFromPlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubePlaylistsRepository youtubePlaylistsRepository,
    IYoutubePlaylistVideosRepository youtubePlaylistVideosRepository
) : ICommandHandler<RemoveVideoFromPlaylistCommand>
{
    public async Task<Result> Handle(
        RemoveVideoFromPlaylistCommand request,
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

        var item = await youtubePlaylistVideosRepository.GetByPlaylistIdAndYoutubeIdAsync(
            request.YoutubePlaylistId,
            request.YoutubeId,
            cancellationToken
        );

        if (item is null)
        {
            return Result.Failure(YoutubePlaylistErrors.VideoNotInPlaylist(request.YoutubeId));
        }

        youtubePlaylistVideosRepository.Remove(item);

        return Result.Success();
    }
}
