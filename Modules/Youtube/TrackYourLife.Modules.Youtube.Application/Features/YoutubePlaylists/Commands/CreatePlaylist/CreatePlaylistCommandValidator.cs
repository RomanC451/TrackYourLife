using FluentValidation;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.CreatePlaylist;

internal sealed class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
{
    public CreatePlaylistCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(200)
            .WithMessage("Name must be at most 200 characters.");
    }
}
