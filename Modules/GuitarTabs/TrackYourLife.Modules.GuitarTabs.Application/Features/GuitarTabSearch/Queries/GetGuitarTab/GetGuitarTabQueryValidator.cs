using FluentValidation;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.GetGuitarTab;

internal sealed class GetGuitarTabQueryValidator : AbstractValidator<GetGuitarTabQuery>
{
    public GetGuitarTabQueryValidator()
    {
        RuleFor(x => x.Url)
            .NotEmpty()
            .WithMessage("A tab URL is required.")
            .MaximumLength(500)
            .Must(url => UltimateGuitarTabUrl.TryCreate(url, out _))
            .WithMessage("Only a public Ultimate Guitar tab page can be opened.");
    }
}
