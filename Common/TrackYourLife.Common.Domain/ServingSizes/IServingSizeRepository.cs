namespace TrackYourLife.Common.Domain.ServingSizes;

public interface IServingSizeRepository
{
    Task<ServingSize?> GetByIdAsync(ServingSizeId id, CancellationToken cancellationToken);
}
