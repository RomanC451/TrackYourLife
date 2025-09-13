using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;

internal class UpdateRecipeDiaryCommandValidator : AbstractValidator<UpdateRecipeDiaryCommand>
{
    public UpdateRecipeDiaryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
        RuleFor(x => x.MealType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.ServingSizeId).NotEmptyId();
    }
}
