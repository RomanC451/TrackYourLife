using TrackYourLife.Modules.Nutrition.Contracts.Dtos;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Contracts.MappingsExtensions;

/// <summary>
/// Represents a extension class for mapping between different types related to foods.
/// </summary>

public static class FoodMappingsExtensions
{
    public static ServingSizeDto ToDto(this ServingSizeReadModel servingSize)
    {
        return new ServingSizeDto(
            servingSize.Id,
            servingSize.NutritionMultiplier,
            servingSize.Value,
            servingSize.Unit
        );
    }

    public static ServingSizeDto ToDto(this ServingSize servingSize)
    {
        return new ServingSizeDto(
            servingSize.Id,
            servingSize.NutritionMultiplier,
            servingSize.Value,
            servingSize.Unit
        );
    }

    public static FoodDto ToDto(this FoodReadModel food)
    {
        return new FoodDto(
            food.Id,
            food.Type,
            food.BrandName,
            food.CountryCode,
            food.Name,
            food.NutritionalContents,
            food.FoodServingSizes.OrderBy(fss => fss.Index)
                .Select(fss => fss.ServingSize.ToDto())
                .ToList()
        );
    }
}
