using TrackYourLife.Modules.Nutrition.Application.Features.RecipeDiaries.Commands.UpdateRecipeDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.RecipesDiaries.Commands;

internal sealed record UpdateRecipeDiaryRequest(
    float Quantity,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    DateOnly EntryDate
);

internal sealed class UpdateRecipeDiary(ISender sender)
    : Endpoint<UpdateRecipeDiaryRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}");
        Group<RecipeDiariesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateRecipeDiaryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new UpdateRecipeDiaryCommand(
                Id: Route<NutritionDiaryId>("id")!,
                Quantity: req.Quantity,
                MealType: req.MealType,
                ServingSizeId: req.ServingSizeId,
                EntryDate: req.EntryDate
            ))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
