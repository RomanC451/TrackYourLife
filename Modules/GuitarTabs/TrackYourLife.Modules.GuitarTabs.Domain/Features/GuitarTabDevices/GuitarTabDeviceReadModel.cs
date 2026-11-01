using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

public sealed record GuitarTabDeviceReadModel(
    GuitarTabDeviceId Id,
    UserId UserId,
    string Name,
    decimal Speed,
    int Semitones,
    bool UseFlats,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<GuitarTabDeviceId>;
