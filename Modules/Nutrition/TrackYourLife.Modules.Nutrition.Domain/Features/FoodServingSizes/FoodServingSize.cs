using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;

public sealed class FoodServingSize
{
    public FoodId FoodId { get; set; } = FoodId.Empty;
    public ServingSizeId ServingSizeId { get; set; } = ServingSizeId.Empty;
    public int Index { get; set; }

    private FoodServingSize() { }

    private FoodServingSize(FoodId foodId, ServingSizeId servingSizeId, int index)
    {
        FoodId = foodId;
        ServingSizeId = servingSizeId;
        Index = index;
    }

    public static Result<FoodServingSize> Create(
        FoodId foodId,
        ServingSizeId servingSizeId,
        int index
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                foodId,
                DomainErrors.ArgumentError.Empty(nameof(FoodServingSize), nameof(foodId))
            ),
            Ensure.NotEmptyId(
                servingSizeId,
                DomainErrors.ArgumentError.Empty(nameof(FoodServingSize), nameof(servingSizeId))
            ),
            Ensure.NotNegative(
                index,
                DomainErrors.ArgumentError.Negative(nameof(FoodServingSize), nameof(index))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<FoodServingSize>(result.Error);
        }

        return Result.Success(new FoodServingSize(foodId, servingSizeId, index));
    }
}
