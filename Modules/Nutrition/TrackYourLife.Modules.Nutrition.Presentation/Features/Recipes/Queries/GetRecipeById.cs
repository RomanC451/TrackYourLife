using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.Recipes.Queries.GetRecipeById;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Recipes.Queries;

public class GetRecipeById(ISender sender, INutritionMapper mapper)
    : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("{id}");
        Group<RecipesGroup>();
        Description(x =>
            x.Produces<RecipeDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetRecipeByIdQuery(Route<RecipeId>("id")!))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync((recipe) => TypedResults.Ok(mapper.Map<RecipeDto>(recipe)));
    }
}
