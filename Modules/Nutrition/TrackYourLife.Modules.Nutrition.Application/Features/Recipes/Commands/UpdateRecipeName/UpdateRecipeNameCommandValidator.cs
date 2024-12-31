using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipeName;

public sealed class UpdateRecipeNameCommandValidator : AbstractValidator<UpdateRecipeNameCommand>
{
    public UpdateRecipeNameCommandValidator()
    {
        RuleFor(x => x.RecipeId).NotEmptyId();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
