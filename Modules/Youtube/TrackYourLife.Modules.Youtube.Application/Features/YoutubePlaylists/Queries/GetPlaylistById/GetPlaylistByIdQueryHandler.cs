using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylistById;

internal sealed class GetPlaylistByIdQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubePlaylistsQuery youtubePlaylistsQuery,
    IYoutubeApiService youtubeApiService,
    IWatchedVideosRepository watchedVideosRepository
) : IQueryHandler<GetPlaylistByIdQuery, GetPlaylistByIdResult>
{
    public async Task<Result<GetPlaylistByIdResult>> Handle(
        GetPlaylistByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var playlist = await youtubePlaylistsQuery.GetByIdAndUserIdAsync(
            request.YoutubePlaylistId,
            userId,
            cancellationToken
        );

        if (playlist is null)
        {
            return Result.Failure<GetPlaylistByIdResult>(
                YoutubePlaylistErrors.NotFound(request.YoutubePlaylistId.Value)
            );
        }

        var videos = await youtubePlaylistsQuery.GetVideosByPlaylistIdOrderedAsync(
            request.YoutubePlaylistId,
            cancellationToken
        );

        var uniqueVideoIds = videos.Select(v => v.YoutubeId).Distinct().ToList();

        var detailsByVideoId = new Dictionary<string, YoutubeVideoDetails>();
        foreach (var vid in uniqueVideoIds)
        {
            var result = await youtubeApiService.GetVideoDetailsAsync(vid, cancellationToken);
            if (result.IsSuccess)
            {
                detailsByVideoId[vid] = result.Value;
            }
        }

        HashSet<string> watchedSet;
        if (uniqueVideoIds.Count == 0)
        {
            watchedSet = [];
        }
        else
        {
            var watchedVideos = await watchedVideosRepository.GetByUserIdAndVideoIdsAsync(
                userId,
                uniqueVideoIds,
                cancellationToken
            );
            watchedSet = new HashSet<string>(watchedVideos.Select(w => w.VideoId));
        }

        var withPreviews = videos
            .Select(v =>
            {
                YoutubeVideoPreview? preview = null;
                if (detailsByVideoId.TryGetValue(v.YoutubeId, out var details))
                {
                    preview = YoutubeVideoPreviewMapper.FromDetails(
                        details,
                        watchedSet.Contains(v.YoutubeId)
                    );
                }

                return new YoutubePlaylistVideoWithPreview(v, preview);
            })
            .ToList();

        return Result.Success(
            new GetPlaylistByIdResult(
                playlist,
                withPreviews.OrderByDescending(v => v.Video.AddedOnUtc).ToList().AsReadOnly()
            )
        );
    }
}
