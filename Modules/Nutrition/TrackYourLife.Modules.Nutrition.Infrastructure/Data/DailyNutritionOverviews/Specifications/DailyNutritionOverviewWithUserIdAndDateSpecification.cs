using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.DailyNutritionOverviews;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.DailyNutritionOverviews.Specifications;

public sealed class DailyNutritionOverviewWithUserIdAndDateSpecification(
    UserId userId,
    DateOnly date
) : Specification<DailyNutritionOverview, DailyNutritionOverviewId>
{
    public override Expression<Func<DailyNutritionOverview, bool>> ToExpression() =>
        overview => overview.UserId == userId && overview.Date == date;
}

public sealed class DailyNutritionOverviewReadModelWithUserIdAndDateSpecification(
    UserId userId,
    DateOnly date
) : Specification<DailyNutritionOverviewReadModel, DailyNutritionOverviewId>
{
    public override Expression<Func<DailyNutritionOverviewReadModel, bool>> ToExpression() =>
        overview => overview.UserId == userId && overview.Date == date;
}
