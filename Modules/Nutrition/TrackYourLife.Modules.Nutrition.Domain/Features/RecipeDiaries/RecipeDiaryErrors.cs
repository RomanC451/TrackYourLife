using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.RecipeDiaries;

public static class RecipeDiaryErrors
{
    public static readonly Func<NutritionDiaryId, Error> NotFound = id =>
        Error.NotFound(id, arg2: nameof(RecipeDiary));

    public static readonly Func<NutritionDiaryId, Error> NotOwned = (id) =>
        Error.NotOwned(id, nameof(RecipeDiary));
}
