using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetAllLatestVideos;

internal sealed class GetAllLatestVideosQueryValidator : AbstractValidator<GetAllLatestVideosQuery>
{
    public GetAllLatestVideosQueryValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .When(x => x.Category.HasValue)
            .WithMessage("Invalid video category.");

        RuleFor(x => x.MaxResultsPerChannel)
            .InclusiveBetween(1, 20)
            .WithMessage("Max results per channel must be between 1 and 20.");
    }
}

