using FluentValidation;
using TrackYourLife.SharedLib.Application.Extensions;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipes;

public sealed class DeleteRecipesCommandValidator : AbstractValidator<DeleteRecipesCommand>
{
    public DeleteRecipesCommandValidator()
    {
        RuleForEach(x => x.Ids).NotEmptyId();
        RuleFor(x => x.Ids).Must(ids => ids.Any()).WithMessage("Ids must not be empty");
        RuleFor(x => x.Ids)
            .Must(ids => ids.Distinct().Count() == ids.Count())
            .WithMessage("Ids must be unique");
    }
}
