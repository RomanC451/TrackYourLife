using FluentValidation;
using TrackYourLife.Modules.Nutrition.Application.Features.DailyNutritionOverviews.Queries.GetDailyNutritionOverviewsByDateRange;

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

        RuleFor(x => x.EndDate)
            .Must(
                (query, endDate) =>
                    (
                        endDate.ToDateTime(TimeOnly.MinValue)
                        - query.StartDate.ToDateTime(TimeOnly.MinValue)
                    ).Days <= 365
            )
            .WithMessage("Date range cannot exceed 365 days.");
    }
}
