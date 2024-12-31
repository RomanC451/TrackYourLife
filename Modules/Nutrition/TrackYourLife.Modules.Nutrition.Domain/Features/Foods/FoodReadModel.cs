using NpgsqlTypes;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

public sealed record FoodReadModel(
    FoodId Id,
    string Name,
    string Type,
    string BrandName,
    string CountryCode
) : IReadModel<FoodId>
{
    public List<FoodServingSizeReadModel> FoodServingSizes { get; init; } = [];
    public NutritionalContent NutritionalContents { get; init; } = new();
    public NpgsqlTsVector SearchVector { get; init; } = null!;
}
