using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.UpdateRecipeName;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record UpdateRecipeNameRequest(string Name);

internal sealed class UpdateRecipeName(ISender sender) : Endpoint<UpdateRecipeNameRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}/name");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateRecipeNameRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new UpdateRecipeNameCommand(Route<RecipeId>("id")!, req.Name))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
