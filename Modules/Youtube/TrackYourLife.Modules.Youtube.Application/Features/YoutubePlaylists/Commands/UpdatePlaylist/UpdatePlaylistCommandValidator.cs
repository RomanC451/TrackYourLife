using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.UpdatePlaylist;

internal sealed class UpdatePlaylistCommandValidator : AbstractValidator<UpdatePlaylistCommand>
{
    public UpdatePlaylistCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name must be at most 200 characters.");
    }
}
