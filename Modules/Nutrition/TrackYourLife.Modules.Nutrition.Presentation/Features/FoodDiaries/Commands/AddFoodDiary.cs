using TrackYourLife.Modules.Nutrition.Application.Features.FoodDiaries.Commands.AddFoodDiary;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.FoodDiaries.Commands;

internal sealed record AddFoodDiaryRequest(
    FoodId FoodId,
    MealTypes MealType,
    ServingSizeId ServingSizeId,
    float Quantity,
    DateOnly EntryDate
);

internal sealed class AddFoodDiary(ISender sender) : Endpoint<AddFoodDiaryRequest, IResult>
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
            .Create(
                new AddFoodDiaryCommand(
                    req.FoodId,
                    req.MealType,
                    req.ServingSizeId,
                    req.Quantity,
                    req.EntryDate
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.FoodDiaries}/{id.Value}");
    }
}
