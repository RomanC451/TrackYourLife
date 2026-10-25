using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetYoutubeVideoDuration;

internal sealed class GetYoutubeVideoDurationQueryValidator
    : AbstractValidator<GetYoutubeVideoDurationQuery>
{
    public GetYoutubeVideoDurationQueryValidator()
    {
        RuleFor(x => x.VideoId)
            .NotEmpty()
            .Matches("^[A-Za-z0-9_-]{11}$")
            .WithMessage("Video ID is required.");
    }
}
