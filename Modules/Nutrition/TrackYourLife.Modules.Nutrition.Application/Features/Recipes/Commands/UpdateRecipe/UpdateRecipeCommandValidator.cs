using FluentValidation;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipe;

public sealed class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeCommandValidator(
        IRecipeQuery recipeQuery,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        RuleFor(x => x.RecipeId).NotEmptyId();
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(
                async (command, name, cancellationToken) =>
                {
                    var existingRecipe = await recipeQuery.GetByNameAndUserIdAsync(
                        name,
                        userIdentifierProvider.UserId,
                        cancellationToken
                    );
                    return existingRecipe is null || existingRecipe.Id == command.RecipeId;
                }
            )
            .WithMessage("Recipe with this name already exists");
        RuleFor(x => x.Portions).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThan(0);
    }
}
