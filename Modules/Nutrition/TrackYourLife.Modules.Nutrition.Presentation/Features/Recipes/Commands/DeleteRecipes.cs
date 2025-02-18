using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.DeleteRecipes;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record DeleteRecipesRequest(IEnumerable<RecipeId> Ids);

internal sealed class DeleteRecipes(ISender sender) : Endpoint<DeleteRecipesRequest, IResult>
{
    public override void Configure()
    {
        Delete("batch");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .Produces(StatusCodes.Status404NotFound)
                .Produces(StatusCodes.Status400BadRequest)
                .ProducesProblemFE(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(DeleteRecipesRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new DeleteRecipesCommand(req.Ids))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
