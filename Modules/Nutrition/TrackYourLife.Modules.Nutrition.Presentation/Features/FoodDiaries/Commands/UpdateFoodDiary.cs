using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.UpdateFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

internal sealed record UpdateFoodDiaryRequest(
    NutritionDiaryId Id,
    float Quantity,
    ServingSizeId ServingSizeId,
    MealTypes MealType
);

internal sealed class UpdateFoodDiary(ISender sender, INutritionMapper mapper)
    : Endpoint<UpdateFoodDiaryRequest, IResult>
{
    public override void Configure()
    {
        Put("");
        Group<FoodDiariesGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateFoodDiaryRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(mapper.Map<UpdateFoodDiaryCommand>)
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
