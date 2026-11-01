using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.CreateGuitarTabDevice;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarTabDevices.Commands;

internal sealed record CreateGuitarTabDeviceRequest(
    Guid DeviceId,
    string Name,
    decimal Speed,
    int Semitones,
    bool UseFlats
);

internal sealed class CreateGuitarTabDevice(ISender sender)
    : Endpoint<CreateGuitarTabDeviceRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<GuitarTabDevicesGroup>();
        Description(builder =>
            builder
                .Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        CreateGuitarTabDeviceRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new CreateGuitarTabDeviceCommand(
                    req.DeviceId,
                    req.Name,
                    req.Speed,
                    req.Semitones,
                    req.UseFlats
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.GuitarTabDevices}/{id.Value}");
    }
}
