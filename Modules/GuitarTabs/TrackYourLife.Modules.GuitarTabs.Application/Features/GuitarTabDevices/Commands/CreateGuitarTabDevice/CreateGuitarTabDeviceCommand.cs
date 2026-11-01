using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.CreateGuitarTabDevice;

public sealed record CreateGuitarTabDeviceCommand(
    Guid DeviceId,
    string Name,
    decimal Speed,
    int Semitones,
    bool UseFlats
) : ICommand<GuitarTabDeviceId>;
