namespace TrackYourLife.Common.Domain.Foods;

public interface IFoodRepository
{
    Task<Food?> GetByIdAsync(FoodId id, CancellationToken cancellationToken);
    Task<List<Food>> GetFoodListByContainingNameAsync(
        string name,
        CancellationToken cancellationToken
    );
    Task AddAsync(Food food, CancellationToken cancellationToken);
    Task AddFoodListAsync(List<Food> foodList, CancellationToken cancellationToken);
    void Remove(Food food);
}
