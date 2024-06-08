using Microsoft.Extensions.Caching.Memory;
using TrackYourLife.Common.Domain.Foods;
using TrackYourLife.Common.Domain.Repositories;

namespace TrackYourLife.Common.Persistence.Repositories;

internal sealed class CachedFoodRepository(IMemoryCache memoryCache, IFoodRepository decorated) : IFoodRepository, ICachedRepository
{
    private readonly IFoodRepository _decorated = decorated;
    private readonly IMemoryCache memoryCache = memoryCache;

    public Task AddAsync(Food food, CancellationToken cancellationToken) => _decorated.AddAsync(food, cancellationToken);

    public Task AddFoodListAsync(List<Food> foodList, CancellationToken cancellationToken) => _decorated.AddFoodListAsync(foodList, cancellationToken);

    public Task<Food?> GetByIdAsync(FoodId id, CancellationToken cancellationToken)
    {
        string key = $"Food_{id}";

        return memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            return await _decorated.GetByIdAsync(id, cancellationToken);
        });
    }

    public async Task<List<Food>> GetFoodListByContainingNameAsync(string name, CancellationToken cancellationToken)
    {
        string key = $"FoodList_{name}";

        var list = await memoryCache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

            return await _decorated.GetFoodListByContainingNameAsync(name, cancellationToken);
        });

        return list ?? new List<Food>();
    }

    public void Remove(Food food)
    {
        _decorated.Remove(food);
    }
}
