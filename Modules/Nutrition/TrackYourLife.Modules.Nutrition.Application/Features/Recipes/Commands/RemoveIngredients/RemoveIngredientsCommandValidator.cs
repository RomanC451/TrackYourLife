using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredients;

public sealed class RemoveIngredientsCommandValidator : AbstractValidator<RemoveIngredientsCommand>
{
    public RemoveIngredientsCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmptyId();
        RuleForEach(x => x.IngredientsIds).NotEmptyId();
        RuleFor(x => x.IngredientsIds).Must(ids => ids.Distinct().Count() == ids.Count);
        RuleFor(x => x.IngredientsIds).NotEmpty();
    }
}
