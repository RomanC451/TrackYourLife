using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.AddVideoToPlaylist;

internal sealed class AddVideoToPlaylistCommandValidator
    : AbstractValidator<AddVideoToPlaylistCommand>
{
    public AddVideoToPlaylistCommandValidator()
    {
        RuleFor(x => x.VideoId)
            .NotEmpty()
            .WithMessage("YouTube video ID is required.")
            .MaximumLength(50)
            .WithMessage("YouTube video ID must be at most 50 characters.");
    }
}
