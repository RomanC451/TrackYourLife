using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarTabDevices;

internal sealed class GuitarTabDevicesRepository(GuitarTabsWriteDbContext dbContext)
    : GenericRepository<GuitarTabDevice, GuitarTabDeviceId>(dbContext.GuitarTabDevices),
        IGuitarTabDevicesRepository
{
    public async Task<GuitarTabDevice?> GetByIdAndUserIdAsync(
        GuitarTabDeviceId id,
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.GuitarTabDevices.FirstOrDefaultAsync(
            device => device.Id == id && device.UserId == userId,
            cancellationToken
        );
    }

    public Task<bool> NameExistsAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = name.ToLower();
        return dbContext.GuitarTabDevices.AnyAsync(
            device => device.UserId == userId && device.Name.ToLower() == normalized,
            cancellationToken
        );
    }
}
