using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;

/// <summary>
/// Represents a command to update a food diary entry.
/// </summary>
public sealed record UpdateFoodDiaryCommand(
    NutritionDiaryId Id,
    float Quantity,
    ServingSizeId ServingSizeId,
    MealTypes MealType,
    DateOnly EntryDate
) : ICommand;
