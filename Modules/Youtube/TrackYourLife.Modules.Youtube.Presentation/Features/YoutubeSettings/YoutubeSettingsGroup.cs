using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeSettings;

internal sealed class YoutubeSettingsGroup : Group
{
    public YoutubeSettingsGroup()
    {
        Configure(
            ApiRoutes.Settings,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
