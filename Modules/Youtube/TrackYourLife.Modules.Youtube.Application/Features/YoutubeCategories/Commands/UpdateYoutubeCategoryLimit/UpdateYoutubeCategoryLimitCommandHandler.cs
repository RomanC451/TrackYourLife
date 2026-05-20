using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.UpdateYoutubeCategoryLimit;

internal sealed class UpdateYoutubeCategoryLimitCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubeCategoriesRepository youtubeCategoriesRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdateYoutubeCategoryLimitCommand>
{
    public async Task<Result> Handle(
        UpdateYoutubeCategoryLimitCommand request,
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

        var maxResult = category.UpdateMaxVideosPerDay(request.MaxVideosPerDay, utcNow);
        if (maxResult.IsFailure)
        {
            return maxResult;
        }

        youtubeCategoriesRepository.Update(category);
        return Result.Success();
    }
}
