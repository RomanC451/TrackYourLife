using NpgsqlTypes;
using TrackYourLife.Modules.Nutrition.Domain.Features.FoodServingSizes;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

public sealed class Food : Entity<FoodId>
{
    public override FoodId Id { get; set; } = FoodId.Empty;
    public string Type { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public long? ApiId { get; set; } = null;
    public string Name { get; set; } = string.Empty;
    public NutritionalContent NutritionalContents { get; set; } = new();
    public ICollection<FoodServingSize> FoodServingSizes { get; set; } = [];

    public NpgsqlTsVector SearchVector { get; init; } = null!;

    private Food()
        : base() { }

    private Food(
        FoodId id,
        string type,
        string brandName,
        string countryCode,
        string name,
        NutritionalContent nutritionalContents,
        ICollection<FoodServingSize> foodServingSizes,
        long? apiId
    )
        : base(id)
    {
        Type = type;
        BrandName = brandName;
        CountryCode = countryCode;
        Name = name;
        NutritionalContents = nutritionalContents;
        FoodServingSizes = foodServingSizes;
        ApiId = apiId;
    }

    public static Result<Food> Create(
        FoodId id,
        string type,
        string brandName,
        string countryCode,
        string name,
        NutritionalContent nutritionalContents,
        ICollection<FoodServingSize> foodServingSizes,
        long? apiId = null
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Food), nameof(id))),
            Ensure.NotEmpty(type, DomainErrors.ArgumentError.Empty(nameof(Food), nameof(type))),
            Ensure.NotEmpty(name, DomainErrors.ArgumentError.Empty(nameof(Food), nameof(name))),
            Ensure.NotNull(
                nutritionalContents,
                DomainErrors.ArgumentError.Null(nameof(Food), nameof(nutritionalContents))
            ),
            Ensure.NotEmptyList(
                foodServingSizes,
                DomainErrors.ArgumentError.Empty(nameof(Food), nameof(foodServingSizes))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Food>(result.Error);
        }

        return Result.Success(
            new Food(
                id,
                type,
                brandName,
                countryCode,
                name,
                nutritionalContents,
                foodServingSizes,
                apiId
            )
        );
    }

    public bool HasServingSize(ServingSizeId servingSizeId)
    {
        return FoodServingSizes.Any(x => x.ServingSizeId == servingSizeId);
    }
}
