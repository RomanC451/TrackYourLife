using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Commands.MoveChannelToCategory;

internal sealed class MoveChannelToCategoryCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ISubscriptionStatusProvider subscriptionStatusProvider,
    IYoutubeChannelsRepository youtubeChannelsRepository,
    IYoutubeCategoriesQuery youtubeCategoriesQuery,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<MoveChannelToCategoryCommand>
{
    private const int FreeUserMaxCategories = 2;

    public async Task<Result> Handle(
        MoveChannelToCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var channel = await youtubeChannelsRepository.GetByYoutubeChannelIdAsync(
            request.YoutubeChannelId,
            cancellationToken
        );

        if (channel is null || channel.UserId != userId)
        {
            return Result.Failure(YoutubeChannelsErrors.NotFound(request.YoutubeChannelId));
        }

        if (channel.YoutubeCategoryId == request.TargetYoutubeCategoryId)
        {
            return Result.Success();
        }

        var category = await youtubeCategoriesQuery.GetByIdAsync(
            request.TargetYoutubeCategoryId,
            cancellationToken
        );

        if (category is null || category.UserId != userId)
        {
            return Result.Failure(YoutubeCategoriesErrors.NotFound);
        }

        var categories = await youtubeCategoriesQuery.ListByUserIdOrderedAsync(
            userId,
            cancellationToken
        );
        var isPro = await subscriptionStatusProvider.IsProAsync(cancellationToken);

        if (!isPro && categories.Count > FreeUserMaxCategories)
        {
            var allowed = categories
                .OrderBy(c => c.DisplayOrder)
                .Take(FreeUserMaxCategories)
                .Select(c => c.Id)
                .ToHashSet();

            if (!allowed.Contains(request.TargetYoutubeCategoryId))
            {
                return Result.Failure(YoutubeCategoriesErrors.CannotAssignChannelToCategory);
            }
        }

        var assign = channel.AssignCategory(
            request.TargetYoutubeCategoryId,
            category.Name,
            dateTimeProvider.UtcNow
        );

        if (assign.IsFailure)
        {
            return Result.Failure(assign.Error);
        }

        youtubeChannelsRepository.Update(channel);

        return Result.Success();
    }
}
