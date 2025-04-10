using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries.Specifications;

internal sealed class RecipeDiaryReadModelWithUserIdAndRecipeIdSpecification(
    UserId userId,
    RecipeId recipeId
) : Specification<RecipeDiaryReadModel, NutritionDiaryId>
{
    public override Expression<Func<RecipeDiaryReadModel, bool>> ToExpression() =>
        recipeDiary => recipeDiary.UserId == userId && recipeDiary.Recipe.Id == recipeId;
}
