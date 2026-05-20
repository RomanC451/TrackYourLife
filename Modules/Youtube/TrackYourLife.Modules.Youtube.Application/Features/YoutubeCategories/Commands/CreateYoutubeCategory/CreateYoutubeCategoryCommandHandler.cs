using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Options;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.CreateYoutubeCategory;

internal sealed class CreateYoutubeCategoryCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ISubscriptionStatusProvider subscriptionStatusProvider,
    IYoutubeCategoriesRepository youtubeCategoriesRepository,
    IOptions<YoutubeModuleOptions> moduleOptions,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateYoutubeCategoryCommand, YoutubeCategoryId>
{
    private const int FreeUserMaxCategories = 2;

    public async Task<Result<YoutubeCategoryId>> Handle(
        CreateYoutubeCategoryCommand request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var isPro = await subscriptionStatusProvider.IsProAsync(cancellationToken);
        var count = await youtubeCategoriesRepository.CountByUserIdAsync(userId, cancellationToken);

        var maxAllowed = isPro ? moduleOptions.Value.MaxCategoriesForPro : FreeUserMaxCategories;

        if (count >= maxAllowed)
        {
            return Result.Failure<YoutubeCategoryId>(YoutubeCategoriesErrors.ForbiddenForPlan);
        }

        if (
            await youtubeCategoriesRepository.ExistsByUserIdAndNameIgnoreCaseAsync(
                userId,
                request.Name,
                excludeId: null,
                cancellationToken
            )
        )
        {
            return Result.Failure<YoutubeCategoryId>(YoutubeCategoriesErrors.DuplicateName);
        }

        var existing = await youtubeCategoriesRepository.ListByUserIdOrderedAsync(userId, cancellationToken);
        var nextOrder = existing.Count == 0 ? 0 : existing.Max(c => c.DisplayOrder) + 1;

        var id = YoutubeCategoryId.NewId();
        var create = YoutubeCategory.Create(
            id,
            userId,
            request.Name,
            maxVideosPerDay: request.MaxVideosPerDay,
            displayOrder: nextOrder,
            createdOnUtc: dateTimeProvider.UtcNow
        );

        if (create.IsFailure)
        {
            return Result.Failure<YoutubeCategoryId>(create.Error);
        }

        await youtubeCategoriesRepository.AddAsync(create.Value, cancellationToken);

        return Result.Success(id);
    }
}
