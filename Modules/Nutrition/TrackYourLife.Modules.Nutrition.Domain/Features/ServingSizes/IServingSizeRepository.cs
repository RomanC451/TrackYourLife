namespace TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

public interface IServingSizeRepository
{
    Task<ServingSize?> GetByIdAsync(ServingSizeId id, CancellationToken cancellationToken);

    Task<ServingSize?> GetByApiIdAsync(long apiId, CancellationToken cancellationToken);

    Task<List<ServingSize>> GetWhereApiIdPartOfListAsync(
        List<long> apiIds,
        CancellationToken cancellationToken
    );

    Task AddRangeAsync(IEnumerable<ServingSize> servingSizes, CancellationToken cancellationToken);
}
