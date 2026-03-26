using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.CreatePlaylist;

internal sealed class CreatePlaylistCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubePlaylistsRepository youtubePlaylistsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreatePlaylistCommand, YoutubePlaylistId>
{
    public async Task<Result<YoutubePlaylistId>> Handle(
        CreatePlaylistCommand request,
        CancellationToken cancellationToken
    )
    {
        var id = YoutubePlaylistId.NewId();
        var createResult = YoutubePlaylist.Create(
            id,
            userIdentifierProvider.UserId,
            request.Name,
            dateTimeProvider.UtcNow
        );

        if (createResult.IsFailure)
        {
            return Result.Failure<YoutubePlaylistId>(createResult.Error);
        }

        await youtubePlaylistsRepository.AddAsync(createResult.Value, cancellationToken);

        return Result.Success(id);
    }
}
