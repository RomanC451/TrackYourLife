using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries;

/// <summary>
/// Represents the extension class for mapping between different types related to recipe diaries.
/// </summary>

internal static class RecipeDiaryMappingsExtensions
{
    public static RecipeDiaryDto ToDto(this RecipeDiaryReadModel recipeDiary)
    {
        return new RecipeDiaryDto(
            recipeDiary.Id,
            recipeDiary.Recipe.ToDto(),
            recipeDiary.MealType,
            recipeDiary.Quantity,
            recipeDiary.Date
        );
    }
}
