using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;

public sealed record UpdateRecipeDiaryCommand(
    NutritionDiaryId Id,
    float Quantity,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    DateOnly EntryDate
) : ICommand;
