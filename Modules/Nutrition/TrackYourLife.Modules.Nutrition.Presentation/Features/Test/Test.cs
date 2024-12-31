using TrackYourLife.Modules.Nutrition.Application.Core.Abstraction;
using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Presentation.Features.Test;

public class Test(INutritionMapper nutritionMapper) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Post("/test");
        AllowAnonymous();
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        await Task.CompletedTask;

        var a = new FoodDiaryReadModel(
            NutritionDiaryId.NewId(),
            UserId.NewId(),
            1.0f,
            MealTypes.Breakfast,
            DateOnly.FromDateTime(DateTime.Now),
            DateTime.Now
        )
        {
            Food = new FoodReadModel(FoodId.NewId(), "Food", "Description", "d", "d"),
            ServingSize = new ServingSizeReadModel(ServingSizeId.NewId(), 1.0f, "g", 100, null)
        };

        var b = nutritionMapper.Map<FoodDiaryDto>(a);

        return TypedResults.Ok(b);
    }
}
