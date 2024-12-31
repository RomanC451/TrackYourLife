using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Nutrition.Domain.Features.ServingSizes;
using TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes.Specifications;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Nutrition.Infrastructure.Data.ServingSizes;

internal sealed class ServingSizeRepository(NutritionWriteDbContext context)
    : GenericRepository<ServingSize, ServingSizeId>(context.ServingSizes),
        IServingSizeRepository
{
    public async Task<ServingSize?> GetByApiIdAsync(long apiId, CancellationToken cancellationToken)
    {
        return await FirstOrDefaultAsync(
            new ServingSizeWithApiIdSpecification(apiId),
            cancellationToken
        );
    }

    public async Task<List<ServingSize>> GetWhereApiIdPartOfListAsync(
        List<long> apiIds,
        CancellationToken cancellationToken
    )
    {
        return await query
            .Where(f => f.ApiId.HasValue && apiIds.Contains(f.ApiId.Value))
            .ToListAsync(cancellationToken);
    }
}
