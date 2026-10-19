using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabYoutube;

internal sealed class SetGuitarTabYoutubeCommandValidator : AbstractValidator<SetGuitarTabYoutubeCommand>
{
    public SetGuitarTabYoutubeCommandValidator()
    {
        RuleFor(command => command.TabUrl)
            .NotEmpty()
            .MaximumLength(SavedGuitarTab.TabUrlMaxLength)
            .Must(url => UltimateGuitarTabUrl.TryCreate(url, out _));
        RuleFor(command => command.SongName).NotEmpty().MaximumLength(SavedGuitarTab.SongNameMaxLength);
        RuleFor(command => command.ArtistName).NotEmpty().MaximumLength(SavedGuitarTab.ArtistNameMaxLength);
        RuleFor(command => command.Type).NotEmpty().MaximumLength(SavedGuitarTab.TypeMaxLength);
        RuleFor(command => command.YoutubeUrl)
            .MaximumLength(500)
            .Must(url => string.IsNullOrWhiteSpace(url) || YoutubeVideoUrl.TryNormalize(url, out _))
            .When(command => command.YoutubeUrl is not null);
    }
}
