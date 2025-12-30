using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetLatestVideosFromChannel;

internal sealed class GetLatestVideosFromChannelQueryHandler(IYoutubeApiService youtubeApiService)
    : IQueryHandler<GetLatestVideosFromChannelQuery, IEnumerable<YoutubeVideoPreview>>
{
    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> Handle(
        GetLatestVideosFromChannelQuery request,
        CancellationToken cancellationToken
    )
    {
        return await youtubeApiService.GetChannelVideosAsync(
            request.ChannelId,
            request.MaxResults,
            cancellationToken
        );
    }
}

