using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;

public sealed record UpdateRecipeDiaryCommand(
    NutritionDiaryId Id,
    float Quantity,
    MealTypes MealType
) : ICommand;
