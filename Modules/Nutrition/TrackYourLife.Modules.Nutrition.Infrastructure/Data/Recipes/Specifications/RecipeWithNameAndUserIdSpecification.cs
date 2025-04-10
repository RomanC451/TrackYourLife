using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Recipes.Specifications;

internal sealed class RecipeWithNameAndUserIdSpecification(string name, UserId userId)
    : Specification<Recipe, RecipeId>
{
    public override Expression<Func<Recipe, bool>> ToExpression() =>
        recipe => recipe.Name == name && recipe.UserId == userId;
}

internal sealed class RecipeReadModelUserIdAndContainingNameSpecification(
    string name,
    UserId userId
) : Specification<RecipeReadModel, RecipeId>
{
    public override Expression<Func<RecipeReadModel, bool>> ToExpression() =>
        recipe => recipe.Name.ToLower().Contains(name.ToLower()) && recipe.UserId == userId;
}
