using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.CreatePlaylist;

internal sealed class CreatePlaylistCommandValidator : AbstractValidator<CreatePlaylistCommand>
{
    public CreatePlaylistCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(GuitarPlaylist.NameMaxLength)
            .WithMessage($"Name must be at most {GuitarPlaylist.NameMaxLength} characters.");
    }
}
