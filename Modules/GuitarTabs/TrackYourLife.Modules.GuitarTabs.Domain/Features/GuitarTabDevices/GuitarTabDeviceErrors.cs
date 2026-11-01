using TrackYourLife.SharedLib.Domain.Errors;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

public static class GuitarTabDeviceErrors
{
    public static readonly Error AlreadySaved =
        new(
            "GuitarTabDevices.AlreadySaved",
            "This device is already saved.",
            409
        );

    public static readonly Error NameAlreadyUsed =
        new(
            "GuitarTabDevices.NameAlreadyUsed",
            "A device with this name already exists.",
            409
        );

    public static readonly Func<Guid, Error> NotFound = deviceId =>
        new(
            "GuitarTabDevices.NotFound",
            $"Device with id '{deviceId}' was not found.",
            404
        );
}
