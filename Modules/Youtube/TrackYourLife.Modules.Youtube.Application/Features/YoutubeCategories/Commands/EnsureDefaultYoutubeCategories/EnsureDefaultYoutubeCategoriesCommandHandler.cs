using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeCategories.Commands.EnsureDefaultYoutubeCategories;

internal sealed class EnsureDefaultYoutubeCategoriesCommandHandler(
    IYoutubeCategoriesRepository youtubeCategoriesRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<EnsureDefaultYoutubeCategoriesCommand>
{
    public async Task<Result> Handle(
        EnsureDefaultYoutubeCategoriesCommand request,
        CancellationToken cancellationToken
    )
    {
        var count = await youtubeCategoriesRepository.CountByUserIdAsync(request.UserId, cancellationToken);

        if (count > 0)
        {
            return Result.Success();
        }

        var utcNow = dateTimeProvider.UtcNow;

        var entId = YoutubeCategoryId.NewId();
        var entResult = YoutubeCategory.Create(
            entId,
            request.UserId,
            YoutubeCategoryDefaults.EntertainmentName,
            YoutubeCategoryDefaults.EntertainmentMaxVideosPerDay,
            YoutubeCategoryDefaults.EntertainmentDisplayOrder,
            utcNow
        );

        if (entResult.IsFailure)
        {
            return Result.Failure(entResult.Error);
        }

        var eduId = YoutubeCategoryId.NewId();
        var eduResult = YoutubeCategory.Create(
            eduId,
            request.UserId,
            YoutubeCategoryDefaults.EducationalName,
            YoutubeCategoryDefaults.EducationalMaxVideosPerDay,
            YoutubeCategoryDefaults.EducationalDisplayOrder,
            utcNow
        );

        if (eduResult.IsFailure)
        {
            return Result.Failure(eduResult.Error);
        }

        await youtubeCategoriesRepository.AddAsync(entResult.Value, cancellationToken);
        await youtubeCategoriesRepository.AddAsync(eduResult.Value, cancellationToken);

        return Result.Success();
    }
}
