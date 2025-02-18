using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;
using TrackYourLife.Modules.Nutrition.Domain.Core;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Services;

internal class RecipeManager(
    IRecipeDiaryQuery recipeDiaryQuery,
    IRecipeDiaryRepository recipeDiaryRepository,
    IQueryRepository recipeRepository,
    IUserIdentifierProvider userIdentifierProvider,
    INutritionUnitOfWork unitOfWork
) : IRecipeManager
{
    public async Task<Result> CloneIfUsed(Recipe recipe, CancellationToken cancellationToken)
    {
        if (
            !await recipeDiaryQuery.AnyByRecipeIdAsync(
                userIdentifierProvider.UserId,
                recipe.Id,
                cancellationToken
            )
        )
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
