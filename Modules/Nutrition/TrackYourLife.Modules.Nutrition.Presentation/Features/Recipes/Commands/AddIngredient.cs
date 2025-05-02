using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.AddIngredient;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record AddIngredientRequest(
    FoodId FoodId,
    ServingSizeId ServingSizeId,
    float Quantity
);

internal sealed class AddIngredient(ISender sender) : Endpoint<AddIngredientRequest, IResult>
{
    public override void Configure()
    {
        Post("{recipeId}/ingredients");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(AddIngredientRequest req, CancellationToken ct)
    {
        var recipeId = Route<RecipeId>("recipeId")!;

        return await Result
            .Create(
                new AddIngredientCommand(
                    RecipeId: Route<RecipeId>("recipeId")!,
                    FoodId: req.FoodId,
                    ServingSizeId: req.ServingSizeId,
                    Quantity: req.Quantity
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(id =>
                TypedResults.Created(
                    $"/{ApiRoutes.Recipes}/{recipeId.Value}/ingredients/{id.Value}",
                    new IdResponse(id)
                )
            );
    }
}
