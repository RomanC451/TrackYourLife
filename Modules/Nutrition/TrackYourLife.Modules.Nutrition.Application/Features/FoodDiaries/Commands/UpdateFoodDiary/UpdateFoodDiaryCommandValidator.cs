using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;

/// <summary>
/// Validates the <see cref="UpdateFoodDiaryCommand"/> before updating a food diary.
/// </summary>
internal class UpdateFoodDiaryCommandValidator : AbstractValidator<UpdateFoodDiaryCommand>
{
    public UpdateFoodDiaryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
        RuleFor(x => x.MealType).IsInEnum();
        RuleFor(x => x.ServingSizeId).NotEmptyId();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
