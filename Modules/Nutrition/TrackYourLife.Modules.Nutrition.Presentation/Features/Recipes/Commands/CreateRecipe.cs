using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record CreateRecipeRequest(string Name);

internal sealed class CreateRecipe(ISender sender) : Endpoint<CreateRecipeRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CreateRecipeRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new CreateRecipeCommand(req.Name))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(recipeId =>
                TypedResults.Created<IdResponse>(
                    $"/{ApiRoutes.Recipes}/{recipeId.Value}",
                    new IdResponse(recipeId)
                )
            );
    }
}
