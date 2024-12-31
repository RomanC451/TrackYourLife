using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;

public sealed record DeleteRecipeDiaryCommand(NutritionDiaryId Id) : ICommand;
