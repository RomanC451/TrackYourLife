using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;

internal sealed class GetLatestVideosFromChannelQueryValidator
    : AbstractValidator<GetLatestVideosFromChannelQuery>
{
    public GetLatestVideosFromChannelQueryValidator()
    {
        RuleFor(x => x.ChannelId)
            .NotEmpty()
            .WithMessage("Channel ID is required.");

        RuleFor(x => x.MaxResults)
            .InclusiveBetween(1, 50)
            .WithMessage("Max results must be between 1 and 50.");
    }
}

