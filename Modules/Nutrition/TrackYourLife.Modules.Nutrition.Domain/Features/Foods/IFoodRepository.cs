namespace TrackYourLife.Modules.Nutrition.Domain.Features.Foods;

public interface IFoodRepository
{
    Task<Food?> GetByIdAsync(FoodId id, CancellationToken cancellationToken);
    Task<Food?> GetByApiIdAsync(long apiId, CancellationToken cancellationToken);
    Task<List<Food>> GetWhereApiIdPartOfListAsync(
        List<long> apiIds,
        CancellationToken cancellationToken
    );
    Task AddAsync(Food food, CancellationToken cancellationToken);
    Task AddRangeAsync(IEnumerable<Food> foodList, CancellationToken cancellationToken);
    void Remove(Food food);
}
