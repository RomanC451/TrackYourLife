namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarTabDevices;

internal sealed class GuitarTabDevicesGroup : Group
{
    public GuitarTabDevicesGroup()
    {
        Configure(
            ApiRoutes.GuitarTabDevices,
            endpoint =>
            {
                endpoint.Description(builder =>
                    builder.ProducesProblem(StatusCodes.Status401Unauthorized)
                );
            }
        );
    }
}
