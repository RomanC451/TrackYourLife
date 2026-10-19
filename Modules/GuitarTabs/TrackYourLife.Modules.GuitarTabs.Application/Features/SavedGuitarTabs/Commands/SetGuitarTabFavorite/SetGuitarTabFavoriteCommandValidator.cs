using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabFavorite;

internal sealed class SetGuitarTabFavoriteCommandValidator : AbstractValidator<SetGuitarTabFavoriteCommand>
{
    public SetGuitarTabFavoriteCommandValidator()
    {
        RuleFor(x => x.TabUrl)
            .NotEmpty()
            .MaximumLength(SavedGuitarTab.TabUrlMaxLength)
            .Must(url => UltimateGuitarTabUrl.TryCreate(url, out _));
        RuleFor(x => x.SongName).NotEmpty().MaximumLength(SavedGuitarTab.SongNameMaxLength);
        RuleFor(x => x.ArtistName).NotEmpty().MaximumLength(SavedGuitarTab.ArtistNameMaxLength);
        RuleFor(x => x.Type).NotEmpty().MaximumLength(SavedGuitarTab.TypeMaxLength);
    }
}
