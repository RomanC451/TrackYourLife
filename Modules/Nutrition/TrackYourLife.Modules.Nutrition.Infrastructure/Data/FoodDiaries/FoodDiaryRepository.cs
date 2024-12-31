using TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.FoodDiaries;

internal sealed class FoodDiaryRepository(NutritionWriteDbContext context)
    : GenericRepository<FoodDiary, NutritionDiaryId>(context.FoodDiaries),
        IFoodDiaryRepository { }
