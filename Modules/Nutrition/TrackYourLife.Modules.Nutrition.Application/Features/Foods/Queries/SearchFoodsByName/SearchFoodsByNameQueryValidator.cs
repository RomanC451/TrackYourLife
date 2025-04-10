using FluentValidation;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Foods.Queries.SearchFoodsByName;

public sealed class SearchFoodsByNameQueryValidator : AbstractValidator<SearchFoodsByNameQuery>
{
    public SearchFoodsByNameQueryValidator()
    {
        RuleFor(x => x.SearchParam).NotEmpty().MaximumLength(100);

        RuleFor(x => x.Page).GreaterThan(0);

        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
    }
}
