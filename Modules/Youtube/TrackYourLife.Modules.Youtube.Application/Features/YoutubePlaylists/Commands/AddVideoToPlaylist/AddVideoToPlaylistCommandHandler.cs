using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.AddVideoToPlaylist;

internal sealed class AddVideoToPlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubePlaylistsRepository youtubePlaylistsRepository,
    IYoutubePlaylistVideosRepository youtubePlaylistVideosRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<AddVideoToPlaylistCommand>
{
    public async Task<Result> Handle(
        AddVideoToPlaylistCommand request,
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

        var exists = await youtubePlaylistVideosRepository.ExistsAsync(
            request.YoutubePlaylistId,
            request.VideoId,
            cancellationToken
        );

        if (exists)
        {
            return Result.Failure(YoutubePlaylistErrors.VideoAlreadyInPlaylist(request.VideoId));
        }

        var itemResult = YoutubePlaylistVideo.Create(
            YoutubePlaylistVideoId.NewId(),
            request.YoutubePlaylistId,
            request.VideoId,
            dateTimeProvider.UtcNow
        );

        if (itemResult.IsFailure)
        {
            return itemResult;
        }

        await youtubePlaylistVideosRepository.AddAsync(itemResult.Value, cancellationToken);

        return Result.Success();
    }
}
