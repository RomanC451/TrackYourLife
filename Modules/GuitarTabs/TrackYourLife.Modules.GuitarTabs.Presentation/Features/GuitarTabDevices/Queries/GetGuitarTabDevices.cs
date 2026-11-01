using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Models;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Queries.GetGuitarTabDevices;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarTabDevices.Queries;

internal sealed class GetGuitarTabDevices(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<GuitarTabDevicesGroup>();
        Description(builder =>
            builder.Produces<IEnumerable<GuitarTabDeviceResponse>>(StatusCodes.Status200OK)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetGuitarTabDevicesQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(devices => devices);
    }
}
