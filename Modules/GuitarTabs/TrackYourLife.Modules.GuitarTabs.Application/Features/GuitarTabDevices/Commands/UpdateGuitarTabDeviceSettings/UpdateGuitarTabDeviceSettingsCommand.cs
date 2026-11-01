using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.UpdateGuitarTabDeviceSettings;

public sealed record UpdateGuitarTabDeviceSettingsCommand(
    GuitarTabDeviceId DeviceId,
    decimal Speed,
    int Semitones,
    bool UseFlats
) : ICommand;
