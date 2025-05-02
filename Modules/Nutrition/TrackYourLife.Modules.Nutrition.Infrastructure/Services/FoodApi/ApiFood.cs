using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;

public sealed class ApiFood
{
    public long Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public NutritionalContent NutritionalContents { get; set; } = new();
    public List<ApiServingSize> ServingSizes { get; set; } = [];
}
