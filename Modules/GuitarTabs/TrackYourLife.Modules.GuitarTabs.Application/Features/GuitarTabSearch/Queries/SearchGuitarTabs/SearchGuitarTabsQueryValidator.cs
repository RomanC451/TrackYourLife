using FluentValidation;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.SearchGuitarTabs;

internal sealed class SearchGuitarTabsQueryValidator : AbstractValidator<SearchGuitarTabsQuery>
{
    public const int MaxValueLength = 200;

    public SearchGuitarTabsQueryValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Search text is required.")
            .MaximumLength(MaxValueLength)
            .WithMessage($"Search text must be at most {MaxValueLength} characters.");

        RuleFor(x => x.Page).GreaterThanOrEqualTo(1).WithMessage("Page must be at least 1.");
    }
}
