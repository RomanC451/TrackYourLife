using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

public interface IRecipeService
{
    Task<Result> CloneIfUsed(Recipe recipe, UserId userId, CancellationToken cancellationToken);
}
