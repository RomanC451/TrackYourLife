using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;

/// <summary>
/// Represents a command to add a food diary entry.
/// </summary>
public sealed record AddFoodDiaryCommand(
    FoodId FoodId,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    float Quantity,
    DateOnly EntryDate
) : ICommand<NutritionDiaryId>;
