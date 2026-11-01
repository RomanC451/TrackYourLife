using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

public interface IGuitarTabDevicesRepository
{
    Task<GuitarTabDevice?> GetByIdAsync(
        GuitarTabDeviceId id,
        CancellationToken cancellationToken = default
    );

    Task<GuitarTabDevice?> GetByIdAndUserIdAsync(
        GuitarTabDeviceId id,
        UserId userId,
        CancellationToken cancellationToken = default
    );

    Task<bool> NameExistsAsync(
        UserId userId,
        string name,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(GuitarTabDevice device, CancellationToken cancellationToken = default);

    void Update(GuitarTabDevice device);
}
