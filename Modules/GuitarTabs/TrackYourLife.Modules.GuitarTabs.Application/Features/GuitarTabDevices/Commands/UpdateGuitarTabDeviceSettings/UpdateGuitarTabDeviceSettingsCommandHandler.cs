using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.UpdateGuitarTabDeviceSettings;

internal sealed class UpdateGuitarTabDeviceSettingsCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarTabDevicesRepository guitarTabDevicesRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<UpdateGuitarTabDeviceSettingsCommand>
{
    public async Task<Result> Handle(
        UpdateGuitarTabDeviceSettingsCommand request,
        CancellationToken cancellationToken
    )
    {
        var device = await guitarTabDevicesRepository.GetByIdAndUserIdAsync(
            request.DeviceId,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (device is null)
        {
            return Result.Failure(GuitarTabDeviceErrors.NotFound(request.DeviceId.Value));
        }

        var updateResult = device.UpdateSettings(
            request.Speed,
            request.Semitones,
            request.UseFlats,
            dateTimeProvider.UtcNow
        );

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        guitarTabDevicesRepository.Update(device);

        return Result.Success();
    }
}
