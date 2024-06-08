namespace TrackYourLife.Common.Domain.Foods;

public sealed record FoodReadModel(
    Guid Id,
    string Name,
    string Type,
    string BrandName,
    string CountryCode,
    NutritionalContent NutritionalContent,
    ICollection<FoodServingSize> FoodServingSizes
);
