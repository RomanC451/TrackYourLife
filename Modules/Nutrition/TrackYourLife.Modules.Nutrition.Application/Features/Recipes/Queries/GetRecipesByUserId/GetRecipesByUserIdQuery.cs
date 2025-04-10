using System.Collections.ObjectModel;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByUserId;

public sealed record GetRecipesByUserIdQuery() : IQuery<ReadOnlyCollection<RecipeReadModel>>;
