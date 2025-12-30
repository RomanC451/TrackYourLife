using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;

internal sealed class AddChannelToCategoryCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeChannelsRepository youtubeChannelsRepository,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    IYoutubeApiService youtubeApiService,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<AddChannelToCategoryCommand, YoutubeChannelId>
{
    public async Task<Result<YoutubeChannelId>> Handle(
        AddChannelToCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        // Check if the channel already exists for this user
        var exists = await youtubeChannelsQuery.ExistsByUserIdAndYoutubeChannelIdAsync(
            userIdentifierProvider.UserId,
            request.YoutubeChannelId,
            cancellationToken
        );

        if (exists)
        {
            return Result.Failure<YoutubeChannelId>(
                YoutubeChannelsErrors.AlreadyExists(request.YoutubeChannelId)
            );
        }

        // Get channel info from YouTube API
        var channelInfoResult = await youtubeApiService.GetChannelInfoAsync(
            request.YoutubeChannelId,
            cancellationToken
        );

        if (channelInfoResult.IsFailure)
        {
            return Result.Failure<YoutubeChannelId>(channelInfoResult.Error);
        }

        var channelInfo = channelInfoResult.Value;

        // Create the channel entity
        var channelId = YoutubeChannelId.NewId();
        var channelResult = YoutubeChannel.Create(
            id: channelId,
            userId: userIdentifierProvider.UserId,
            youtubeChannelId: request.YoutubeChannelId,
            name: channelInfo.Name,
            thumbnailUrl: channelInfo.ThumbnailUrl,
            category: request.Category,
            createdOnUtc: dateTimeProvider.UtcNow
        );

        if (channelResult.IsFailure)
        {
            return Result.Failure<YoutubeChannelId>(channelResult.Error);
        }

        await youtubeChannelsRepository.AddAsync(channelResult.Value, cancellationToken);

        return Result.Success(channelId);
    }
}

