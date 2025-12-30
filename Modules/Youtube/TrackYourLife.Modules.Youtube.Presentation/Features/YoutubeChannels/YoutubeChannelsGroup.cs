using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeChannels;

internal sealed class YoutubeChannelsGroup : Group
{
    public YoutubeChannelsGroup()
    {
        Configure(
            ApiRoutes.Channels,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}

