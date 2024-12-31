using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;

public sealed record AddRecipeDiaryCommand(
    RecipeId RecipeId,
    MealTypes MealType,
    float Quantity,
    DateOnly EntryDate
) : ICommand<NutritionDiaryId>;
