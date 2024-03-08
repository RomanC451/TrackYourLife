using TrackYourLifeDotnet.Domain.Foods.StrongTypes;

namespace TrackYourLifeDotnet.Domain.Foods.Repositories;

public interface IServingSizeRepository
{
    Task<ServingSize?> GetByIdAsync(ServingSizeId id, CancellationToken cancellationToken);
}
