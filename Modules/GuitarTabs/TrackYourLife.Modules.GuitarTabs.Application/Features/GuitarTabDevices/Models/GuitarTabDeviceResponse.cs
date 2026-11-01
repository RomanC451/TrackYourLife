namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Models;

public sealed record GuitarTabDeviceResponse(
    Guid DeviceId,
    string Name,
    decimal Speed,
    int Semitones,
    bool UseFlats
);
