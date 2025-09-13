using TrackYourLife.Modules.Common.Presentation.Contracts;

namespace TrackYourLife.Modules.Common.Presentation.Features.Images;

internal sealed class ImagesGroup : Group
{
    public ImagesGroup()
    {
        Configure(
            ApiRoutes.Images,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
