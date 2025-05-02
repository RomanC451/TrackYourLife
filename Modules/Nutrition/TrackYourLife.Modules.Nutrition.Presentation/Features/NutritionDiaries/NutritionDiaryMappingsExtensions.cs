using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.NutritionDiaries;

/// <summary>
/// Represents the extension class for mapping between different types related to nutrition diaries.
/// </summary>

internal static class NutritionDiaryMappingsExtensions
{
    public static NutritionDiaryDto ToDto(this FoodDiaryReadModel foodDiary)
    {
        return new NutritionDiaryDto(
            foodDiary.Id,
            $"{foodDiary.Food.Name} ({foodDiary.Food.BrandName})",
            foodDiary.ServingSize.ToDto(),
            foodDiary.Food.NutritionalContents.MultiplyNutritionalValues(
                foodDiary.ServingSize.NutritionMultiplier * foodDiary.Quantity
            ),
            foodDiary.ServingSize.NutritionMultiplier,
            foodDiary.Quantity,
            foodDiary.MealType,
            DiaryType.FoodDiary,
            foodDiary.Date
        );
    }

    public static NutritionDiaryDto ToDto(this RecipeDiaryReadModel recipeDiary)
    {
        return new NutritionDiaryDto(
            recipeDiary.Id,
            recipeDiary.Recipe.IsOld ? $"{recipeDiary.Recipe.Name} (old)" : recipeDiary.Recipe.Name,
            null,
            recipeDiary.Recipe.NutritionalContents,
            1.0f,
            recipeDiary.Quantity,
            recipeDiary.MealType,
            DiaryType.RecipeDiary,
            recipeDiary.Date
        );
    }
}
