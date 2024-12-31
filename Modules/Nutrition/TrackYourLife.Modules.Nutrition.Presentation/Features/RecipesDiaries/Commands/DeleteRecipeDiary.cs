using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.DeleteRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;

internal sealed class DeleteRecipeDiary(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("{id}");
        Group<RecipeDiariesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await sender
            .Send(new DeleteRecipeDiaryCommand(Route<NutritionDiaryId>("id")!), ct)
            .ToActionResultAsync();
    }
}
