using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;

internal sealed class GetVideoDetailsQueryValidator : AbstractValidator<GetVideoDetailsQuery>
{
    public GetVideoDetailsQueryValidator()
    {
        RuleFor(x => x.VideoId)
            .NotEmpty()
            .WithMessage("Video ID is required.");
    }
}

