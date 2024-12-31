using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

public interface IRecipeDiaryQuery
{
    Task<RecipeDiaryReadModel?> GetByIdAsync(
        NutritionDiaryId id,
        CancellationToken cancellationToken
    );

    public Task<bool> AnyByRecipeIdAsync(
        UserId userId,
        RecipeId recipeId,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<RecipeDiaryReadModel>> GetByDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<RecipeDiaryReadModel>> GetByPeriodAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );
}
