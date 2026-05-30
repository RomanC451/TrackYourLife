using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.DeleteYoutubeCategory;

internal sealed class DeleteYoutubeCategoryCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeCategoriesRepository youtubeCategoriesRepository,
    IYoutubeChannelsRepository youtubeChannelsRepository,
    IYoutubeChannelsQuery youtubeChannelsQuery,
    IYoutubeCategoriesQuery youtubeCategoriesQuery,
    ISubscriptionStatusProvider subscriptionStatusProvider,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<DeleteYoutubeCategoryCommand>
{
    private const int FreeUserMaxCategories = 2;

    public async Task<Result> Handle(
        DeleteYoutubeCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var category = await youtubeCategoriesRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null || category.UserId != userId)
        {
            return Result.Failure(YoutubeCategoriesErrors.NotFound);
        }

        var channelCount = await youtubeChannelsQuery.CountByUserIdAndYoutubeCategoryIdAsync(
            userId,
            request.CategoryId,
            cancellationToken
        );

        if (channelCount > 0)
        {
            var confirmUnsubscribe = request.ConfirmUnsubscribeChannels;
            var moveToCategoryId = request.MoveChannelsToCategoryId;

            if (confirmUnsubscribe && moveToCategoryId is not null)
            {
                return Result.Failure(YoutubeCategoriesErrors.InvalidDeleteChannelDisposition);
            }

            if (!confirmUnsubscribe && moveToCategoryId is null)
            {
                return Result.Failure(YoutubeCategoriesErrors.DeleteRequiresConfirmation);
            }

            if (confirmUnsubscribe)
            {
                await youtubeChannelsRepository.RemoveAllByUserIdAndCategoryIdAsync(
                    userId,
                    request.CategoryId,
                    cancellationToken
                );
            }
            else
            {
                var moveResult = await MoveChannelsToCategoryAsync(
                    userId,
                    request.CategoryId,
                    moveToCategoryId!,
                    cancellationToken
                );

                if (moveResult.IsFailure)
                {
                    return moveResult;
                }
            }
        }

        youtubeCategoriesRepository.Remove(category);

        return Result.Success();
    }

    private async Task<Result> MoveChannelsToCategoryAsync(
        UserId userId,
        YoutubeCategoryId sourceCategoryId,
        YoutubeCategoryId targetCategoryId,
        CancellationToken cancellationToken
    )
    {
        if (sourceCategoryId == targetCategoryId)
        {
            return Result.Failure(YoutubeCategoriesErrors.MoveChannelsToSameCategory);
        }

        var targetCategory = await youtubeCategoriesQuery.GetByIdAsync(targetCategoryId, cancellationToken);

        if (targetCategory is null || targetCategory.UserId != userId)
        {
            return Result.Failure(YoutubeCategoriesErrors.NotFound);
        }

        var categories = await youtubeCategoriesQuery.ListByUserIdOrderedAsync(userId, cancellationToken);
        var isPro = await subscriptionStatusProvider.IsProAsync(cancellationToken);

        if (!isPro && categories.Count > FreeUserMaxCategories)
        {
            var allowed = categories
                .OrderBy(c => c.DisplayOrder)
                .Take(FreeUserMaxCategories)
                .Select(c => c.Id)
                .ToHashSet();

            if (!allowed.Contains(targetCategoryId))
            {
                return Result.Failure(YoutubeCategoriesErrors.CannotAssignChannelToCategory);
            }
        }

        var channels = await youtubeChannelsRepository.ListByUserIdAndCategoryIdAsync(
            userId,
            sourceCategoryId,
            cancellationToken
        );

        var utcNow = dateTimeProvider.UtcNow;

        foreach (var channel in channels)
        {
            var assign = channel.AssignCategory(targetCategoryId, targetCategory.Name, utcNow);

            if (assign.IsFailure)
            {
                return Result.Failure(assign.Error);
            }

            youtubeChannelsRepository.Update(channel);
        }

        return Result.Success();
    }
}
