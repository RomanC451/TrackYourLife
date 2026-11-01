using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.UpdateGuitarTabDeviceSettings;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarTabDevices.Commands;

internal sealed record UpdateGuitarTabDeviceSettingsRequest(
    decimal Speed,
    int Semitones,
    bool UseFlats
);

internal sealed class UpdateGuitarTabDeviceSettings(ISender sender)
    : Endpoint<UpdateGuitarTabDeviceSettingsRequest, IResult>
{
    public override void Configure()
    {
        Put("{deviceId:guid}/settings");
        Group<GuitarTabDevicesGroup>();
        Description(builder =>
            builder
                .Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateGuitarTabDeviceSettingsRequest req,
        CancellationToken ct
    )
    {
        var deviceId = GuitarTabDeviceId.Create(Route<Guid>("deviceId"));

        return await Result
            .Create(
                new UpdateGuitarTabDeviceSettingsCommand(
                    deviceId,
                    req.Speed,
                    req.Semitones,
                    req.UseFlats
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
