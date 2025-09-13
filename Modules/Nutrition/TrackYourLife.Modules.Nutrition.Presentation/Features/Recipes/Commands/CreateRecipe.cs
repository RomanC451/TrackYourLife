using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Commands.CreateRecipe;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Commands;

internal sealed record CreateRecipeRequest(string Name, int Portions, float Weight);

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
            .Create(
                new CreateRecipeCommand(Name: req.Name, Portions: req.Portions, Weight: req.Weight)
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(recipeId => $"/{ApiRoutes.Recipes}/{recipeId.Value}");
    }
}
