using FluentValidation;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;

/// <summary>
/// Validates the <see cref="AddFoodDiaryCommand"/> before it is processed.
/// </summary>
internal class AddRecipeDiaryCommandValidator : AbstractValidator<AddRecipeDiaryCommand>
{
    public AddRecipeDiaryCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmptyId();

        RuleFor(x => x.MealType).IsInEnum();

        RuleFor(x => x.Quantity).GreaterThan(0);

        RuleFor(x => x.EntryDate).NotEmpty();

        RuleFor(x => x.ServingSizeId).NotEmptyId();
    }
}
