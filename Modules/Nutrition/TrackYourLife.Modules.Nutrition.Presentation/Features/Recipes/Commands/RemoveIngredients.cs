using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record RemoveIngredientsRequest(List<IngredientId> IngredientsIds);

internal sealed class RemoveIngredients(ISender sender)
    : Endpoint<RemoveIngredientsRequest, IResult>
{
    public override void Configure()
    {
        Delete("{recipeId}/ingredients/");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        RemoveIngredientsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new RemoveIngredientsCommand(Route<RecipeId>("recipeId")!, req.IngredientsIds))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
