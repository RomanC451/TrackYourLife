using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.UpdatePlaylist;

internal sealed class UpdatePlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubePlaylistsRepository youtubePlaylistsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdatePlaylistCommand>
{
    public async Task<Result> Handle(
        UpdatePlaylistCommand request,
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

        var updateResult = playlist.UpdateName(request.Name, dateTimeProvider.UtcNow);
        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        youtubePlaylistsRepository.Update(playlist);

        return Result.Success();
    }
}
