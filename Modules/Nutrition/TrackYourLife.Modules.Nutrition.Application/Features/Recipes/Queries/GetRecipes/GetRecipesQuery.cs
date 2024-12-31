using System.Collections.ObjectModel;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByName;

public sealed record GetRecipesByNameQuery() : IQuery<ReadOnlyCollection<RecipeReadModel>>;
