using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.CreateGuitarTabDevice;

internal sealed class CreateGuitarTabDeviceCommandHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarTabDevicesRepository guitarTabDevicesRepository,
    IDateTimeProvider dateTimeProvider
) : ICommandHandler<CreateGuitarTabDeviceCommand, GuitarTabDeviceId>
{
    public async Task<Result<GuitarTabDeviceId>> Handle(
        CreateGuitarTabDeviceCommand request,
        CancellationToken cancellationToken
    )
    {
        var deviceId = GuitarTabDeviceId.Create(request.DeviceId);
        var existing = await guitarTabDevicesRepository.GetByIdAsync(deviceId, cancellationToken);
        if (existing is not null)
        {
            return Result.Failure<GuitarTabDeviceId>(GuitarTabDeviceErrors.AlreadySaved);
        }

        var name = request.Name.Trim();
        if (await guitarTabDevicesRepository.NameExistsAsync(
                userIdentifierProvider.UserId,
                name,
                cancellationToken
            ))
        {
            return Result.Failure<GuitarTabDeviceId>(GuitarTabDeviceErrors.NameAlreadyUsed);
        }

        var createResult = GuitarTabDevice.Create(
            deviceId,
            userIdentifierProvider.UserId,
            name,
            request.Speed,
            request.Semitones,
            request.UseFlats,
            dateTimeProvider.UtcNow
        );

        if (createResult.IsFailure)
        {
            return Result.Failure<GuitarTabDeviceId>(createResult.Error);
        }

        await guitarTabDevicesRepository.AddAsync(createResult.Value, cancellationToken);

        return Result.Success(deviceId);
    }
}
