using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.DeleteFoodDiary;

/// <summary>
/// Validator for the <see cref="DeleteFoodDiaryCommand"/> class.
/// </summary>
internal class DeleteFoodDiaryCommandValidator : AbstractValidator<DeleteFoodDiaryCommand>
{
    public DeleteFoodDiaryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
    }
}
