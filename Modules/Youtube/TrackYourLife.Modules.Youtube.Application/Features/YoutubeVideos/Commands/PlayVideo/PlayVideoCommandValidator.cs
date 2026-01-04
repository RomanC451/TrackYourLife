using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Commands.PlayVideo;

internal sealed class PlayVideoCommandValidator : AbstractValidator<PlayVideoCommand>
{
    public PlayVideoCommandValidator()
    {
        RuleFor(x => x.VideoId).NotEmpty().WithMessage("Video ID is required.");
    }
}
