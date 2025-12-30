using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetVideoDetails;

internal sealed class GetVideoDetailsQueryHandler(IYoutubeApiService youtubeApiService)
    : IQueryHandler<GetVideoDetailsQuery, YoutubeVideoDetails>
{
    public async Task<Result<YoutubeVideoDetails>> Handle(
        GetVideoDetailsQuery request,
        CancellationToken cancellationToken
    )
    {
        return await youtubeApiService.GetVideoDetailsAsync(request.VideoId, cancellationToken);
    }
}

