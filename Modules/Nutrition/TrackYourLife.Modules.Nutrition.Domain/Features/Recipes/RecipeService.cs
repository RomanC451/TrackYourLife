using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public class RecipeService(
    IRecipeDiaryQuery recipeDiaryQuery,
    IRecipeDiaryRepository recipeDiaryRepository,
    IRecipeRepository recipeRepository,
    INutritionUnitOfWork unitOfWork
) : IRecipeService
{
    public async Task<Result> CloneIfUsed(
        Recipe recipe,
        UserId userId,
        CancellationToken cancellationToken
    )
    {
        if (!await recipeDiaryQuery.AnyByRecipeIdAsync(userId, recipe.Id, cancellationToken))
            return Result.Success();

        var clonedRecipeResult = recipe.Clone(RecipeId.NewId());

        if (clonedRecipeResult.IsFailure)
            return Result.Failure(clonedRecipeResult.Error);

        var clonedRecipe = clonedRecipeResult.Value;

        clonedRecipe.MarkAsOld();

        await recipeRepository.AddAsync(clonedRecipe, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await recipeDiaryRepository.BulkUpdateRecipeId(
            recipe.Id,
            clonedRecipe.Id,
            cancellationToken
        );

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
