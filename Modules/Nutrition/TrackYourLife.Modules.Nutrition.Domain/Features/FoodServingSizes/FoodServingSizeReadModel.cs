using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;

public sealed record FoodServingSizeReadModel(FoodId FoodId, ServingSizeId ServingSizeId, int Index)
{
    public ServingSizeReadModel ServingSize { get; set; } = null!;
}
