using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record UpdateIngredientRequest(ServingSizeId ServingSizeId, float Quantity);

internal sealed class UpdateIngredient(ISender sender) : Endpoint<UpdateIngredientRequest, IResult>
{
    public override void Configure()
    {
        Put("{recipeId}/ingredients/{ingredientId}");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateIngredientRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new UpdateIngredientCommand(
                    RecipeId: Route<RecipeId>("recipeId")!,
                    IngredientId: Route<IngredientId>("ingredientId")!,
                    ServingSizeId: req.ServingSizeId,
                    Quantity: req.Quantity
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
