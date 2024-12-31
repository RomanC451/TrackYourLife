using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public record ServingSizeDto(ServingSizeId Id, float NutritionMultiplier, float Value, string Unit);
