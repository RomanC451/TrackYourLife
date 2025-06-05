using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipesByUserId;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Queries;

internal sealed class GetRecipesByUserId(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces<List<RecipeDto>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetRecipesByUserIdQuery())
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(recipes => recipes.Select(r => r.ToDto()).ToList());
    }
}
