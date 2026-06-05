using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Contracts.Common;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Queries.GetWatchHistory;

internal sealed class GetWatchHistoryQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IWatchedVideosRepository watchedVideosRepository,
    IYoutubeApiService youtubeApiService
) : IQueryHandler<GetWatchHistoryQuery, PagedList<WatchedVideoHistoryEntry>>
{
    public async Task<Result<PagedList<WatchedVideoHistoryEntry>>> Handle(
        GetWatchHistoryQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var (watchedVideos, totalCount) = await watchedVideosRepository.GetPagedByUserIdAsync(
            userId,
            request.Page,
            request.PageSize,
            cancellationToken
        );

        if (watchedVideos.Count == 0)
        {
            return PagedList<WatchedVideoHistoryEntry>.FromSlice(
                [],
                request.Page,
                request.PageSize,
                totalCount
            );
        }

        var videoIds = watchedVideos.Select(w => w.VideoId).ToList();

        var previewsResult = await youtubeApiService.GetVideoPreviewsByIdsAsync(
            videoIds,
            cancellationToken
        );

        if (previewsResult.IsFailure)
        {
            return Result.Failure<PagedList<WatchedVideoHistoryEntry>>(previewsResult.Error);
        }

        var previewsById = previewsResult.Value;

        var entries = watchedVideos
            .Select(w =>
            {
                previewsById.TryGetValue(w.VideoId, out var preview);
                return new WatchedVideoHistoryEntry(preview, w.VideoId, w.WatchedAtUtc);
            })
            .ToList();

        return PagedList<WatchedVideoHistoryEntry>.FromSlice(
            entries,
            request.Page,
            request.PageSize,
            totalCount
        );
    }
}
