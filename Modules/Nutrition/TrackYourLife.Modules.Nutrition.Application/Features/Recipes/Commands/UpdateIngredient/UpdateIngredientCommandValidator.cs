using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateIngredient;

public sealed class UpdateIngredientCommandValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmptyId();
        RuleFor(x => x.IngredientId).NotEmptyId();
        RuleFor(x => x.ServingSizeId).NotEmptyId();
        RuleFor(x => x.Quantity).NotEmpty();
    }
}
