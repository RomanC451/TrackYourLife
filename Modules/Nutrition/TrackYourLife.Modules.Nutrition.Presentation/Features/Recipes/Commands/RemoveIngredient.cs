using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.RemoveIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed class RemoveIngredient(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("{recipeId}/ingredients/{ingredientId}");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(
                new RemoveIngredientCommand(
                    Route<RecipeId>("recipeId")!,
                    Route<IngredientId>("ingredientId")!
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
