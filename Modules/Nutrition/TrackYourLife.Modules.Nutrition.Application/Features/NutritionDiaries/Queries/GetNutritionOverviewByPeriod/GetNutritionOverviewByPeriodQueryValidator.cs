using FluentValidation;

namespace TrackYourLife.Modules.Nutrition.Application.Features.NutritionDiaries.Queries.GetNutritionOverviewByPeriod;

public sealed class GetNutritionOverviewByPeriodQueryValidator
    : AbstractValidator<GetNutritionOverviewByPeriodQuery>
{
    public GetNutritionOverviewByPeriodQueryValidator()
    {
        RuleFor(x => x.StartDate).NotEmpty();

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.StartDate)
            .Must((query, endDate) => endDate.DayNumber - query.StartDate.DayNumber <= 365);
    }
}
