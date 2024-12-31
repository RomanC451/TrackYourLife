using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;

public sealed class AddIngredientCommandValidator : AbstractValidator<AddIngredientCommand>
{
    public AddIngredientCommandValidator()
    {
        RuleFor(x => x.FoodId).NotEmptyId();
        RuleFor(x => x.ServingSizeId).NotEmptyId();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
