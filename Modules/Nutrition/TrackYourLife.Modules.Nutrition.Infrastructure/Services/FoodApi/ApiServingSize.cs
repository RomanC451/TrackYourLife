namespace TrackYourLife.Modules.Nutrition.Infrastructure.Services.FoodApi;

public sealed class ApiServingSize
{
    public long Id { get; set; }
    public float NutritionMultiplier { get; set; }
    public string Unit { get; set; } = string.Empty;
    public float Value { get; set; }
}
