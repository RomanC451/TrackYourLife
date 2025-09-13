using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record UpdateRecipeRequest(string Name, int Portions, float Weight);

internal sealed class UpdateRecipe(ISender sender) : Endpoint<UpdateRecipeRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(UpdateRecipeRequest req, CancellationToken ct)
    {
        return await Result
            .Create(
                new UpdateRecipeCommand(
                    RecipeId: Route<RecipeId>("id")!,
                    Name: req.Name,
                    Portions: req.Portions,
                    Weight: req.Weight
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
