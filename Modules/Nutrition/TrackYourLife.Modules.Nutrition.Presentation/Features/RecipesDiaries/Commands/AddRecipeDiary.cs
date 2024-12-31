using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.AddRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Recipes;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;

internal sealed record AddRecipeDiaryRequest(
    RecipeId RecipeId,
    MealTypes MealType,
    float Quantity,
    DateOnly EntryDate
);

internal sealed class AddRecipeDiary(ISender sender, INutritionMapper mapper)
    : Endpoint<AddRecipeDiaryRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<RecipeDiariesGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        AddRecipeDiaryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<AddRecipeDiaryCommand>)
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(recipe =>
                TypedResults.Created(
                    $"/{ApiRoutes.FoodDiaries}/{recipe.Value}",
                    new IdResponse(recipe)
                )
            );
    }
}
