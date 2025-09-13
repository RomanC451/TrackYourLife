using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

internal sealed record UpdateFoodDiaryRequest(
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    float Quantity,
    DateOnly EntryDate
);

internal sealed class UpdateFoodDiary(ISender sender) : Endpoint<UpdateFoodDiaryRequest, IResult>
{
    public override void Configure()
    {
        Put("{id}");
        Group<FoodDiariesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status403Forbidden)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateFoodDiaryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(req => new UpdateFoodDiaryCommand(
                Id: Route<NutritionDiaryId>("id")!,
                Quantity: req.Quantity,
                ServingSizeId: req.ServingSizeId,
                MealType: req.MealType,
                EntryDate: req.EntryDate
            ))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
