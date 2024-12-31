using TrackYourLife.Modules.Nutrition.Domain.Features.Ingredients;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public sealed record IngredientDto(
    IngredientId Id,
    FoodDto Food,
    ServingSizeDto ServingSize,
    float Quantity
);
