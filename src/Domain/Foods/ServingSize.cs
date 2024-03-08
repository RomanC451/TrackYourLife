using Newtonsoft.Json;
using TrackYourLifeDotnet.Domain.Foods.StrongTypes;
using TrackYourLifeDotnet.Domain.Primitives;

namespace TrackYourLifeDotnet.Domain.Foods;

public class ServingSize : Entity<ServingSizeId>
{
    public double NutritionMultiplier { get; set; } = new();
    public string Unit { get; set; } = string.Empty;
    public double Value { get; set; }
    public long? ApiId { get; set; } = null;

    public ServingSize()
        : base() { }

    public ServingSize(
        ServingSizeId id,
        double nutritionMultiplier,
        string unit,
        double value,
        long? apiId
    )
        : base(id)
    {
        NutritionMultiplier = nutritionMultiplier;
        Unit = unit;
        Value = value;
        ApiId = apiId;
    }
}
