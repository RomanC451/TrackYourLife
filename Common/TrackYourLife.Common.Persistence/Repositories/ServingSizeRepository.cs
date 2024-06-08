using Microsoft.EntityFrameworkCore;
using TrackYourLife.Common.Domain.ServingSizes;

namespace TrackYourLife.Common.Persistence.Repositories;

internal sealed class ServingSizeRepository
    : GenericRepository<ServingSize, ServingSizeId>,
        IServingSizeRepository
{
    public ServingSizeRepository(ApplicationWriteDbContext context)
        : base(context.ServingSizes) { }
}
