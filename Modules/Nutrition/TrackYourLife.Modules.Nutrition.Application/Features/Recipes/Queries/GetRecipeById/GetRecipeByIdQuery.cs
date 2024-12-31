using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipeById;

public sealed record GetRecipeByIdQuery(RecipeId Id) : IQuery<RecipeReadModel>;
