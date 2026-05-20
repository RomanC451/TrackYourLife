using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryMetadata;

internal sealed class UpdateYoutubeCategoryMetadataCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeCategoriesRepository youtubeCategoriesRepository,
    IYoutubeChannelsRepository youtubeChannelsRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdateYoutubeCategoryMetadataCommand>
{
    public async Task<Result> Handle(
        UpdateYoutubeCategoryMetadataCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var utcNow = dateTimeProvider.UtcNow;

        var category = await youtubeCategoriesRepository.GetByIdAsync(request.CategoryId, cancellationToken);

        if (category is null || category.UserId != userId)
        {
            return Result.Failure(YoutubeCategoriesErrors.NotFound);
        }

        if (
            await youtubeCategoriesRepository.ExistsByUserIdAndNameIgnoreCaseAsync(
                userId,
                request.Name,
                excludeId: request.CategoryId,
                cancellationToken
            )
        )
        {
            return Result.Failure(YoutubeCategoriesErrors.DuplicateName);
        }

        var nameResult = category.UpdateName(request.Name, utcNow);
        if (nameResult.IsFailure)
        {
            return nameResult;
        }

        category.UpdateDisplayOrder(request.DisplayOrder, utcNow);
        youtubeCategoriesRepository.Update(category);

        var channels = await youtubeChannelsRepository.ListByUserIdAndCategoryIdAsync(
            userId,
            request.CategoryId,
            cancellationToken
        );

        foreach (var ch in channels)
        {
            ch.SyncCategoryName(category.Name, utcNow);
            youtubeChannelsRepository.Update(ch);
        }

        return Result.Success();
    }
}
