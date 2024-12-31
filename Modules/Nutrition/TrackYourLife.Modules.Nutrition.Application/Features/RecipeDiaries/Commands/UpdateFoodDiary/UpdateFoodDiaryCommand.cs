using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateFoodDiary;

public sealed record UpdateRecipeDiaryCommand(
    NutritionDiaryId Id,
    float Quantity,
    MealTypes MealType
) : ICommand;
