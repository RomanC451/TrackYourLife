using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

public interface IRecipeDiaryRepository
{
    Task<RecipeDiary?> GetByIdAsync(NutritionDiaryId id, CancellationToken cancellationToken);

    Task AddAsync(RecipeDiary recipeDiary, CancellationToken cancellationToken);

    void Remove(RecipeDiary recipeDiary);

    void Update(RecipeDiary recipeDiary);

    public Task BulkUpdateRecipeId(
        RecipeId oldRecipeId,
        RecipeId newRecipeId,
        CancellationToken cancellationToken
    );
}
