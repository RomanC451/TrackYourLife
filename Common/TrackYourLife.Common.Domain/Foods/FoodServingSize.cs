using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Domain.Foods;

public class FoodServingSize
{
    public FoodId FoodId { get; set; } = FoodId.Empty;

    public ServingSizeId ServingSizeId { get; set; } = ServingSizeId.Empty;
    public ServingSize ServingSize { get; set; } = new();

    public int Index { get; set; }

    public FoodServingSize() { }

    public FoodServingSize(
        FoodId foodId,
        ServingSizeId servingSizeId,
        ServingSize servingSize,
        int index
    )
    {
        FoodId = foodId;
        ServingSizeId = servingSizeId;
        ServingSize = servingSize;
        Index = index;
    }
}
