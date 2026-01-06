using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

namespace TrackYourLife.Modules.Nutrition.Contracts.Dtos;

public sealed record FoodDto(
    FoodId Id,
    string Type,
    string BrandName,
    string CountryCode,
    string Name,
    NutritionalContent NutritionalContents,
    List<ServingSizeDto> ServingSizes,
    ServingSizeId? LastServingSizeUsedId = null,
    float? LastQuantityUsed = null
);
