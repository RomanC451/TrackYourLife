using FluentValidation;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;

public sealed class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator(
        IRecipeQuery recipeQuery,
        IUserIdentifierProvider userIdentifierProvider
    )
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100)
            .MustAsync(
                async (name, cancellationToken) =>
                {
                    var existingRecipe = await recipeQuery.GetByNameAndUserIdAsync(
                        name,
                        userIdentifierProvider.UserId,
                        cancellationToken
                    );
                    return existingRecipe is null;
                }
            )
            .WithMessage("Recipe with this name already exists");
        RuleFor(x => x.Portions).GreaterThan(0);
        RuleFor(x => x.Weight).GreaterThan(0);
    }
}
