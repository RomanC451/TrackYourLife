using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarTabDevices;

internal sealed class GuitarTabDevicesQuery(GuitarTabsReadDbContext dbContext) : IGuitarTabDevicesQuery
{
    public async Task<IReadOnlyList<GuitarTabDeviceReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .GuitarTabDevices.AsNoTracking()
            .Where(device => device.UserId == userId)
            .OrderBy(device => device.CreatedOnUtc)
            .ThenBy(device => device.Id)
            .ToListAsync(cancellationToken);
    }
}
