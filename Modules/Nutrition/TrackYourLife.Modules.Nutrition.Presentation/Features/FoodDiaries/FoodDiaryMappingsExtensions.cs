using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Presentation.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries;

/// <summary>
/// Represents the extension class for mapping between different types related to food diaries.
/// </summary>

internal static class FoodDiaryMappingsExtensions
{
    public static FoodDiaryDto ToDto(this FoodDiaryReadModel foodDiary)
    {
        return new FoodDiaryDto(
            foodDiary.Id,
            foodDiary.Food.ToDto(),
            foodDiary.MealType,
            foodDiary.Quantity,
            foodDiary.ServingSize.ToDto(),
            foodDiary.Date
        );
    }
}
