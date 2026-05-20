using FluentValidation;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;

internal sealed class GetAllLatestVideosQueryValidator : AbstractValidator<GetAllLatestVideosQuery>
{
    public GetAllLatestVideosQueryValidator()
    {
        When(
            x => x.YoutubeCategoryId is not null,
            () =>
            {
                RuleFor(x => x.YoutubeCategoryId!).NotEqual(YoutubeCategoryId.Empty).WithMessage("Invalid video category id.");
            }
        );

        RuleFor(x => x.MaxResultsPerChannel)
            .InclusiveBetween(1, 20)
            .WithMessage("Max results per channel must be between 1 and 20.");
    }
}
