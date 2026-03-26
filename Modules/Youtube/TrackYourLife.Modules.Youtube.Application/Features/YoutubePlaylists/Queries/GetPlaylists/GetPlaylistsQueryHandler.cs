using TrackYourLife.Modules.Youtube.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylists;

internal sealed class GetPlaylistsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IYoutubePlaylistsQuery youtubePlaylistsQuery,
    IYoutubeApiService youtubeApiService,
    IWatchedVideosRepository watchedVideosRepository
) : IQueryHandler<GetPlaylistsQuery, IReadOnlyList<YoutubePlaylistWithVideoPreviews>>
{
    private const int MaxVideoPreviewsPerPlaylist = 50;

    public async Task<Result<IReadOnlyList<YoutubePlaylistWithVideoPreviews>>> Handle(
        GetPlaylistsQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;
        var playlists = await youtubePlaylistsQuery.GetByUserIdAsync(userId, cancellationToken);

        if (playlists.Count == 0)
        {
            return Result.Success<IReadOnlyList<YoutubePlaylistWithVideoPreviews>>(
                Array.Empty<YoutubePlaylistWithVideoPreviews>()
            );
        }

        var playlistVideoOrder = new Dictionary<YoutubePlaylistId, List<string>>();
        foreach (var playlistId in playlists.Select(pl => pl.Id))
        {
            var videos = await youtubePlaylistsQuery.GetVideosByPlaylistIdOrderedAsync(
                playlistId,
                cancellationToken
            );
            var ids = videos.Select(v => v.YoutubeId).Take(MaxVideoPreviewsPerPlaylist).ToList();
            playlistVideoOrder[playlistId] = ids;
        }

        var uniqueVideoIds = playlistVideoOrder.Values.SelectMany(x => x).Distinct().ToList();

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

        var output = new List<YoutubePlaylistWithVideoPreviews>();
        foreach (var pl in playlists)
        {
            var orderedIds = playlistVideoOrder[pl.Id];
            var previews = new List<YoutubeVideoPreview>();
            foreach (var videoId in orderedIds)
            {
                if (!detailsByVideoId.TryGetValue(videoId, out var details))
                {
                    continue;
                }

                previews.Add(
                    YoutubeVideoPreviewMapper.FromDetails(details, watchedSet.Contains(videoId))
                );
            }

            output.Add(
                new YoutubePlaylistWithVideoPreviews(
                    pl.Id.Value,
                    pl.Name,
                    pl.CreatedOnUtc,
                    pl.ModifiedOnUtc,
                    previews.AsReadOnly()
                )
            );
        }

        return Result.Success<IReadOnlyList<YoutubePlaylistWithVideoPreviews>>(output);
    }
}
