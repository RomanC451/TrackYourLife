using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Models;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Queries.GetGuitarTabDevices;

public sealed record GetGuitarTabDevicesQuery : IQuery<IReadOnlyList<GuitarTabDeviceResponse>>;
