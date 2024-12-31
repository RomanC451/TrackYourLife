using Microsoft.Extensions.Caching.Memory;
using TrackYourLife.Modules.Nutrition.Domain.Features.Foods;
using TrackYourLife.SharedLib.Domain.Repositories;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.Foods;

internal sealed class CachedFoodQuery(IMemoryCache memoryCache, IFoodQuery decorated)
    : IFoodQuery,
        ICachedRepository
{
    private readonly IFoodQuery _decorated = decorated;
    private readonly IMemoryCache memoryCache = memoryCache;

    public Task<FoodReadModel?> GetByIdAsync(FoodId id, CancellationToken cancellationToken) =>
        _decorated.GetByIdAsync(id, cancellationToken);

    public async Task<IEnumerable<FoodReadModel>> SearchFoodAsync(
        string searchTerm,
        CancellationToken cancellationToken
    )
    {
        string key = $"FoodList_{searchTerm}";

        var list = await memoryCache.GetOrCreateAsync(
            key,
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);

                return (await _decorated.SearchFoodAsync(searchTerm, cancellationToken)).ToList();
            }
        );

        return list ?? [];
    }

    public Task<List<FoodReadModel>> GetFoodsPartOfAsync(
        IEnumerable<FoodId> foodIds,
        CancellationToken cancellationToken
    ) => _decorated.GetFoodsPartOfAsync(foodIds, cancellationToken);
}
