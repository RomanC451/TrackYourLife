using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

/// <summary>
/// Represents a command to remove a food diary.
/// </summary>
namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;

public sealed record DeleteFoodDiaryCommand(NutritionDiaryId Id) : ICommand;
