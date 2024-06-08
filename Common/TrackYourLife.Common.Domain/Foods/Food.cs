using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TrackYourLife.Common.Domain.Primitives;

namespace TrackYourLife.Common.Domain.Foods;

public class Food : Entity<FoodId>
{
    public override FoodId Id { get; set; } = FoodId.Empty;
    public string Type { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Name { get; set; } = string.Empty;
    public NutritionalContent NutritionalContent { get; set; } = new();
    public ICollection<FoodServingSize> FoodServingSizes { get; set; } =
        new List<FoodServingSize>();

    public long? ApiId { get; set; } = null;

    public Food()
        : base() { }

    public Food(
        FoodId id,
        string type,
        string brandName,
        string countryCode,
        string name,
        NutritionalContent nutritionalContent,
        ICollection<FoodServingSize> foodServingSizes,
        long? apiId
    )
        : base(id)
    {
        Type = type;
        BrandName = brandName;
        CountryCode = countryCode;
        Name = name;
        NutritionalContent = nutritionalContent;
        FoodServingSizes = foodServingSizes;
        ApiId = apiId;
    }
}
