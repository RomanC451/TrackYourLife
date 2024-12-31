using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Contracts.Common;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

internal sealed record AddFoodDiaryRequest(
    FoodId FoodId,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    float Quantity,
    DateOnly EntryDate
);

internal sealed class AddFoodDiary(ISender sender, INutritionMapper mapper)
    : Endpoint<AddFoodDiaryRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<FoodDiariesGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(AddFoodDiaryRequest req, CancellationToken ct)
    {
        return await Result
            .Create(mapper.Map<AddFoodDiaryCommand>(req))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(id =>
                TypedResults.Created($"/{ApiRoutes.FoodDiaries}/{id.Value}", new IdResponse(id))
            );
    }
}
