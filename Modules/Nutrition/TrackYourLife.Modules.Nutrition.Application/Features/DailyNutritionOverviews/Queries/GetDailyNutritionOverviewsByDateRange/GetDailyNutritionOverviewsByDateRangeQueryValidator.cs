using FluentValidation;

namespace TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

public sealed class GetDailyNutritionOverviewsByDateRangeQueryValidator
    : AbstractValidator<GetDailyNutritionOverviewsByDateRangeQuery>
{
    public GetDailyNutritionOverviewsByDateRangeQueryValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("End date must be greater than or equal to start date.");

        RuleFor(x => x.OverviewType).IsInEnum();

        RuleFor(x => x.AggregationMode).IsInEnum();
    }
}
