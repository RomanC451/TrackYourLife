using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

public interface IFoodDiaryQuery
{
    Task<FoodDiaryReadModel?> GetByIdAsync(
        NutritionDiaryId id,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<FoodDiaryReadModel>> GetByDateAsync(
        UserId userId,
        DateOnly date,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<FoodDiaryReadModel>> GetByPeriodAsync(
        UserId userId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken
    );
}
