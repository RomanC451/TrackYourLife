using FluentValidation;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;

public sealed class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
