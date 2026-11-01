using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

public interface IGuitarTabDevicesQuery
{
    Task<IReadOnlyList<GuitarTabDeviceReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    );
}
