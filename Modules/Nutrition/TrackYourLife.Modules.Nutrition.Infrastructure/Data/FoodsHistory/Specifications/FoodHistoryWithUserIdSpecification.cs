using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory.Specifications;

internal sealed class FoodHistoryReadModelWithUserIdSpecification(UserId userId)
    : Specification<FoodHistoryReadModel, FoodHistoryId>
{
    public override Expression<Func<FoodHistoryReadModel, bool>> ToExpression() =>
        foodHistory => foodHistory.UserId == userId;
}
