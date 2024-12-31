using System.Linq.Expressions;
using TrackYourLife.Modules.Nutrition.Domain.Features.NutritionDiaries;

namespace TrackYourLife.Modules.Nutrition.Domain.Features.FoodDiaries;

public interface IFoodDiaryRepository
{
    Task<FoodDiary?> GetByIdAsync(NutritionDiaryId id, CancellationToken cancellationToken);
    Task AddAsync(FoodDiary entry, CancellationToken cancellationToken);
    void Remove(FoodDiary entry);
    void Update(FoodDiary entry);
}
