using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodsHistory;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodsHistory.Specifications;

public sealed class FoodHistoryWithUserIdAndFoodIdSpecification(UserId userId, FoodId foodId)
    : Specification<FoodHistory, FoodHistoryId>
{
    public override Expression<Func<FoodHistory, bool>> ToExpression() =>
        foodHistory => foodHistory.UserId == userId && foodHistory.FoodId == foodId;
}
