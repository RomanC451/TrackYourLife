using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;

public sealed record IngredientReadModel(IngredientId Id, float Quantity, DateTime CreatedOnUtc)
    : IReadModel<IngredientId>
{
    public FoodReadModel Food { get; set; } = null!;

    public ServingSizeReadModel ServingSize { get; set; } = null!;
}
