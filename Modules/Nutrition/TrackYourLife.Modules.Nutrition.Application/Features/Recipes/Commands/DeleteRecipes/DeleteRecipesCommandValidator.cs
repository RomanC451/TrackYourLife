using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipes;

public sealed class DeleteRecipesCommandValidator : AbstractValidator<DeleteRecipesCommand>
{
    public DeleteRecipesCommandValidator()
    {
        RuleForEach(x => x.Ids).NotEmptyId();
    }
}
