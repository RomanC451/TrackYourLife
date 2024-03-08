using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Foods.Dtos;

public record FoodDto(
    FoodId Id,
    string Type,
    string BrandName,
    string CountryCode,
    string Name,
    NutritionalContent NutritionalContents,
    List<ServingSizeDto> ServingSizes
);
