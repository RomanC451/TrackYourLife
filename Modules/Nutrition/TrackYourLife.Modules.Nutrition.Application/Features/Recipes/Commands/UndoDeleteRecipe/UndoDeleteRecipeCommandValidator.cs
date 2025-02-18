using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UndoDeleteRecipe;

public sealed class UndoDeleteRecipeCommandValidator : AbstractValidator<UndoDeleteRecipeCommand>
{
    public UndoDeleteRecipeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmptyId();
    }
}
