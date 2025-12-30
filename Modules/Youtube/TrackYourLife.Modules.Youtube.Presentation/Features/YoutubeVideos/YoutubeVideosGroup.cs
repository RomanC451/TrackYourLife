using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeVideos;

internal sealed class YoutubeVideosGroup : Group
{
    public YoutubeVideosGroup()
    {
        Configure(
            ApiRoutes.Videos,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}

