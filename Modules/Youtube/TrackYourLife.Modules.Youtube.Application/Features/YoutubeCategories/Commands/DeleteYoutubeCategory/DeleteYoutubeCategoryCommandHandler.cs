using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.DeleteYoutubeCategory;

internal sealed class DeleteYoutubeCategoryCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeCategoriesRepository youtubeCategoriesRepository,
    IYoutubeChannelsRepository youtubeChannelsRepository,
    IYoutubeChannelsQuery youtubeChannelsQuery
) : ICommandHandler<DeleteYoutubeCategoryCommand>
{
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

        if (channelCount > 0 && !request.ConfirmUnsubscribeChannels)
        {
            return Result.Failure(YoutubeCategoriesErrors.DeleteRequiresConfirmation);
        }

        if (channelCount > 0)
        {
            await youtubeChannelsRepository.RemoveAllByUserIdAndCategoryIdAsync(
                userId,
                request.CategoryId,
                cancellationToken
            );
        }

        youtubeCategoriesRepository.Remove(category);

        return Result.Success();
    }
}
