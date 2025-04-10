using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews.Specifications;

internal sealed class DailyNutritionOverviewWithUserIdAndPeriodSpecification(
    UserId userId,
    DateOnly startDate,
    DateOnly endDate
) : Specification<DailyNutritionOverview, DailyNutritionOverviewId>
{
    public override Expression<Func<DailyNutritionOverview, bool>> ToExpression() =>
        overview =>
            overview.UserId == userId && overview.Date >= startDate && overview.Date <= endDate;
}

internal sealed class DailyNutritionOverviewReadModelWithUserIdAndPeriodSpecification(
    UserId userId,
    DateOnly startDate,
    DateOnly endDate
) : Specification<DailyNutritionOverviewReadModel, DailyNutritionOverviewId>
{
    public override Expression<Func<DailyNutritionOverviewReadModel, bool>> ToExpression() =>
        overview =>
            overview.UserId == userId && overview.Date >= startDate && overview.Date <= endDate;
}
