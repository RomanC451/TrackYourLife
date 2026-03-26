using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.RemoveVideoFromPlaylist;

internal sealed class RemoveVideoFromPlaylistCommandValidator : AbstractValidator<RemoveVideoFromPlaylistCommand>
{
    public RemoveVideoFromPlaylistCommandValidator()
    {
        RuleFor(x => x.YoutubeId)
            .NotEmpty()
            .WithMessage("YouTube video ID is required.")
            .MaximumLength(50)
            .WithMessage("YouTube video ID must be at most 50 characters.");
    }
}
