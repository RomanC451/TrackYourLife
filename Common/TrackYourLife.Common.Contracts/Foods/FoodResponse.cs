using TrackYourLife.Common.Domain.Foods;

namespace TrackYourLife.Common.Contracts.Foods;

public sealed record FoodResponse(
    FoodId Id,
    string Type,
    string BrandName,
    string CountryCode,
    string Name,
    NutritionalContent NutritionalContents,
    List<ServingSizeResponse> ServingSizes
);
