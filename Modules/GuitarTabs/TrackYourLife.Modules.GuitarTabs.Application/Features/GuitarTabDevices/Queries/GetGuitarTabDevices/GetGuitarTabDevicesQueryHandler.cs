using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Models;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Queries.GetGuitarTabDevices;

internal sealed class GetGuitarTabDevicesQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarTabDevicesQuery guitarTabDevicesQuery
) : IQueryHandler<GetGuitarTabDevicesQuery, IReadOnlyList<GuitarTabDeviceResponse>>
{
    public async Task<Result<IReadOnlyList<GuitarTabDeviceResponse>>> Handle(
        GetGuitarTabDevicesQuery request,
        CancellationToken cancellationToken
    )
    {
        var devices = await guitarTabDevicesQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        IReadOnlyList<GuitarTabDeviceResponse> response = devices
            .Select(device => new GuitarTabDeviceResponse(
                device.Id.Value,
                device.Name,
                device.Speed,
                device.Semitones,
                device.UseFlats
            ))
            .ToList();

        return Result.Success(response);
    }
}
