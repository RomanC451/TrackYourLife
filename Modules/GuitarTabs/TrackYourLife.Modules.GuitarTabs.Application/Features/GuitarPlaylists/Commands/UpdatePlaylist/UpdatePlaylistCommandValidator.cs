using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.UpdatePlaylist;

internal sealed class UpdatePlaylistCommandValidator : AbstractValidator<UpdatePlaylistCommand>
{
    public UpdatePlaylistCommandValidator()
    {
        RuleFor(x => x.GuitarPlaylistId).NotEqual(GuitarPlaylistId.Empty);

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(GuitarPlaylist.NameMaxLength)
            .WithMessage($"Name must be at most {GuitarPlaylist.NameMaxLength} characters.");
    }
}
