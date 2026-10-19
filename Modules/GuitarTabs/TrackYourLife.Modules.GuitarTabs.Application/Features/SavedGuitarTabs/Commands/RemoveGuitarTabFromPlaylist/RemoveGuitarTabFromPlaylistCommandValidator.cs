using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.RemoveGuitarTabFromPlaylist;

internal sealed class RemoveGuitarTabFromPlaylistCommandValidator
    : AbstractValidator<RemoveGuitarTabFromPlaylistCommand>
{
    public RemoveGuitarTabFromPlaylistCommandValidator()
    {
        RuleFor(x => x.GuitarPlaylistId).NotEqual(GuitarPlaylistId.Empty);
        RuleFor(x => x.SavedGuitarTabId).NotEqual(SavedGuitarTabId.Empty);
    }
}
