using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.RecipeDiaries;

internal class RecipeDiaryRepository(NutritionWriteDbContext context)
    : GenericRepository<RecipeDiary, NutritionDiaryId>(context.RecipeDiaries),
        IRecipeDiaryRepository
{
    public async Task BulkUpdateRecipeId(
        RecipeId oldRecipeId,
        RecipeId newRecipeId,
        CancellationToken cancellationToken
    )
    {
        await query
            .Where(d => d.RecipeId == oldRecipeId)
            .ExecuteUpdateAsync(
                d => d.SetProperty(r => r.RecipeId, newRecipeId),
                cancellationToken
            );
    }
}
