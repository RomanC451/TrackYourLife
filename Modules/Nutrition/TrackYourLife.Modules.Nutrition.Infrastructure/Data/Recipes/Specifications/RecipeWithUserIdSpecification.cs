using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes.Specifications;

internal sealed class RecipeReadModelUserIdSpecification(UserId userId)
    : Specification<RecipeReadModel, RecipeId>
{
    public override Expression<Func<RecipeReadModel, bool>> ToExpression() =>
        recipe => recipe.UserId == userId;
}
