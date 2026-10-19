using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Infrastructure.Options;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Validators;

internal sealed class UltimateGuitarOptionsValidator : AbstractValidator<UltimateGuitarOptions>
{
    public UltimateGuitarOptionsValidator()
    {
        RuleFor(x => x.SearchBaseUrl)
            .NotEmpty()
            .Must(value => Uri.TryCreate(value, UriKind.Absolute, out _))
            .WithMessage("Ultimate Guitar search base URL must be a valid absolute URL.");
    }
}
