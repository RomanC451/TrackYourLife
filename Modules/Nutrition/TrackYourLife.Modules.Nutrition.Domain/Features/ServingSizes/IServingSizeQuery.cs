namespace TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;

public interface IServingSizeQuery
{
    Task<ServingSizeReadModel?> GetByIdAsync(ServingSizeId id, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(ServingSizeId id, CancellationToken cancellationToken);
}
