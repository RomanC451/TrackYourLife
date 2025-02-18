using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredient;

public sealed class RemoveIngredientCommandValidator : AbstractValidator<RemoveIngredientCommand>
{
    public RemoveIngredientCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmptyId();
        RuleForEach(x => x.IngredientsIds).NotEmptyId();
    }
}
