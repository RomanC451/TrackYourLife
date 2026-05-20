using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.AddChannelToCategory;

internal sealed class AddChannelToCategoryCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ISubscriptionStatusProvider subscriptionStatusProvider,
    IYoutubeChannelsRepository youtubeChannelsRepository,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    IYoutubeCategoriesQuery youtubeCategoriesQuery,
    IYoutubeApiService youtubeApiService,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<AddChannelToCategoryCommand, YoutubeChannelId>
{
    private const int FreeUserMaxCategories = 2;

    public async Task<Result<YoutubeChannelId>> Handle(
        AddChannelToCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var exists = await youtubeChannelsQuery.ExistsByUserIdAndYoutubeChannelIdAsync(
            userId,
            request.YoutubeChannelId,
            cancellationToken
        );

        if (exists)
        {
            return Result.Failure<YoutubeChannelId>(
                YoutubeChannelsErrors.AlreadyExists(request.YoutubeChannelId)
            );
        }

        var category = await youtubeCategoriesQuery.GetByIdAsync(
            request.YoutubeCategoryId,
            cancellationToken
        );

        if (category is null || category.UserId != userId)
        {
            return Result.Failure<YoutubeChannelId>(YoutubeCategoriesErrors.NotFound);
        }

        var categories = await youtubeCategoriesQuery.ListByUserIdOrderedAsync(userId, cancellationToken);
        var isPro = await subscriptionStatusProvider.IsProAsync(cancellationToken);

        if (!isPro && categories.Count > FreeUserMaxCategories)
        {
            var allowed = categories.OrderBy(c => c.DisplayOrder).Take(FreeUserMaxCategories).Select(c => c.Id).ToHashSet();

            if (!allowed.Contains(request.YoutubeCategoryId))
            {
                return Result.Failure<YoutubeChannelId>(YoutubeCategoriesErrors.CannotAssignChannelToCategory);
            }
        }

        var channelInfoResult = await youtubeApiService.GetChannelInfoAsync(
            request.YoutubeChannelId,
            cancellationToken
        );

        if (channelInfoResult.IsFailure)
        {
            return Result.Failure<YoutubeChannelId>(channelInfoResult.Error);
        }

        var channelInfo = channelInfoResult.Value;

        var channelId = YoutubeChannelId.NewId();
        var channelResult = YoutubeChannel.Create(
            id: channelId,
            userId: userId,
            youtubeChannelId: request.YoutubeChannelId,
            name: channelInfo.Name,
            thumbnailUrl: channelInfo.ThumbnailUrl,
            youtubeCategoryId: request.YoutubeCategoryId,
            categoryName: category.Name,
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
