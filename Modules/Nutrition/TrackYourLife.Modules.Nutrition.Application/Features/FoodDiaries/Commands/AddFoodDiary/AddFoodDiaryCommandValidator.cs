using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;

/// <summary>
/// Validates the <see cref="AddFoodDiaryCommand"/> before it is processed.
/// </summary>
internal class AddFoodDiaryCommandValidator : AbstractValidator<AddFoodDiaryCommand>
{
    public AddFoodDiaryCommandValidator()
    {
        RuleFor(x => x.FoodId).NotEmptyId();

        RuleFor(x => x.MealType).IsInEnum();

        RuleFor(x => x.ServingSizeId).NotEmptyId();

        RuleFor(x => x.Quantity).GreaterThan(0);

        RuleFor(x => x.EntryDate).NotEmpty();
    }
}
