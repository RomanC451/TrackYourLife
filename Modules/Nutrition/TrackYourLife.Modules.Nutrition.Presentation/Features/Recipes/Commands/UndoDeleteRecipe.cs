using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UndoDeleteRecipe;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record UndoDeleteRecipeRequest(RecipeId Id);

internal sealed class UndoDeleteRecipe(ISender sender) : Endpoint<UndoDeleteRecipeRequest, IResult>
{
    public override void Configure()
    {
        Post("undo-delete");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest)
                .ProducesProblemFE(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UndoDeleteRecipeRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new UndoDeleteRecipeCommand(req.Id))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
