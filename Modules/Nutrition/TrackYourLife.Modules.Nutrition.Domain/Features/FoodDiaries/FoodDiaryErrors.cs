using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

public static class FoodDiaryErrors
{
    public static readonly Func<NutritionDiaryId, Error> NotFound = id =>
        Error.NotFound(id, arg2: nameof(FoodDiary));

    public static readonly Func<NutritionDiaryId, Error> NotOwned = (id) =>
        Error.NotOwned(id, nameof(FoodDiary));
}
