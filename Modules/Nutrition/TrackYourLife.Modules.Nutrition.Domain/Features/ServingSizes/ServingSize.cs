using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

public sealed class ServingSize : Entity<ServingSizeId>
{
    public float NutritionMultiplier { get; set; } = new();
    public string Unit { get; set; } = string.Empty;
    public float Value { get; set; }
    public long? ApiId { get; set; } = null;

    private ServingSize()
        : base() { }

    private ServingSize(
        ServingSizeId id,
        float nutritionMultiplier,
        string unit,
        float value,
        long? apiId
    )
        : base(id)
    {
        NutritionMultiplier = nutritionMultiplier;
        Unit = unit;
        Value = value;
        ApiId = apiId;
    }

    public static Result<ServingSize> Create(
        ServingSizeId id,
        float nutritionMultiplier,
        string unit,
        float value,
        long? apiId = null
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(ServingSize), nameof(id))
            ),
            Ensure.NotNegative(
                nutritionMultiplier,
                DomainErrors.ArgumentError.Negative(
                    nameof(ServingSize),
                    nameof(nutritionMultiplier)
                )
            ),
            Ensure.NotEmpty(
                unit,
                DomainErrors.ArgumentError.Empty(nameof(ServingSize), nameof(unit))
            ),
            Ensure.NotNegative(
                value,
                DomainErrors.ArgumentError.Negative(nameof(ServingSize), nameof(value))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<ServingSize>(result.Error);
        }

        var servingSize = new ServingSize(id, nutritionMultiplier, unit, value, apiId);

        return Result.Success(servingSize);
    }
}
