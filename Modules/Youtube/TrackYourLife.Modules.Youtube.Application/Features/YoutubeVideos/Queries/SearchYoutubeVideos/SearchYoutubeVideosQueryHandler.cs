using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.SearchYoutubeVideos;

internal sealed class SearchYoutubeVideosQueryHandler(IYoutubeApiService youtubeApiService)
    : IQueryHandler<SearchYoutubeVideosQuery, IEnumerable<YoutubeVideoPreview>>
{
    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> Handle(
        SearchYoutubeVideosQuery request,
        CancellationToken cancellationToken
    )
    {
        return await youtubeApiService.SearchVideosAsync(
            request.Query,
            request.MaxResults,
            cancellationToken
        );
    }
}

