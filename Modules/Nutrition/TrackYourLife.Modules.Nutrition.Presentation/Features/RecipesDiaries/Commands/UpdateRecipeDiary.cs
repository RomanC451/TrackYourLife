using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;

internal sealed record UpdateRecipeDiaryRequest(
    NutritionDiaryId Id,
    float Quantity,
    MealTypes MealType
);

internal sealed class UpdateRecipeDiary(ISender sender)
    : Endpoint<UpdateRecipeDiaryRequest, IResult>
{
    public override void Configure()
    {
        Put("");
        Group<RecipeDiariesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateRecipeDiaryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new UpdateRecipeDiaryCommand(req.Id, req.Quantity, req.MealType))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
