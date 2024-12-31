using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Core.Abstraction.Services;

public interface IRecipeManager
{
    Task<Result> CloneIfUsed(Recipe recipe, CancellationToken cancellationToken);
}
