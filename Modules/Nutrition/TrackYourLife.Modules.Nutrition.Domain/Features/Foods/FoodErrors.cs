using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

public static class FoodErrors
{
    public static readonly Func<string, Error> NotFoundByName = name => new Error(
        "Food.NotFound",
        $"The food with the name '{name}' was not found.",
        404
    );

    public static readonly Func<FoodId, Error> NotFoundById = id =>
        Error.NotFound(id, nameof(Food));

    public static readonly Func<int, int, Error> PageOutOfIndex = (page, maxPage) =>
        new Error(
            "Food.PageOutOfIndex",
            $"The page number '{page}' is out of index. Page number must be greater than 0 and smaller than {maxPage + 1}."
        );

    public static readonly Func<FoodId, ServingSizeId, Error> ServingSizeNotFound = (
        foodId,
        servingSizeId
    ) =>
        new Error(
            "Food.ServingSizeNotFound",
            $"Food '{foodId.Value}' doesn't have serving size '{servingSizeId.Value
            }'."
        );
}
