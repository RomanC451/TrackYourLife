using System.Xml;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Microsoft.Extensions.Options;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;
using TrackYourLife.Modules.Youtube.Infrastructure.Options;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Infrastructure.Services;

internal sealed class YoutubeApiService : IYoutubeApiService, IDisposable
{
    private readonly YouTubeService _youtubeService;

    public YoutubeApiService(IOptions<YoutubeApiOptions> options)
    {
        _youtubeService = new YouTubeService(
            new BaseClientService.Initializer
            {
                ApiKey = options.Value.ApiKey,
                ApplicationName = "TrackYourLife",
            }
        );
    }

    public async Task<Result<IEnumerable<YoutubeChannelSearchResult>>> SearchChannelsAsync(
        string query,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var searchRequest = _youtubeService.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.Type = "channel";
            searchRequest.MaxResults = maxResults;

            var searchResponse = await searchRequest.ExecuteAsync(cancellationToken);

            var channelIds = searchResponse.Items.Select(item => item.Snippet.ChannelId).ToList();

            if (channelIds.Count == 0)
            {
                return Result.Success(Enumerable.Empty<YoutubeChannelSearchResult>());
            }

            // Get channel statistics
            var channelsRequest = _youtubeService.Channels.List("snippet,statistics");
            channelsRequest.Id = string.Join(",", channelIds);

            var channelsResponse = await channelsRequest.ExecuteAsync(cancellationToken);

            var results = channelsResponse.Items.Select(channel => new YoutubeChannelSearchResult(
                ChannelId: channel.Id,
                Name: channel.Snippet.Title,
                Description: channel.Snippet.Description ?? string.Empty,
                ThumbnailUrl: channel.Snippet.Thumbnails?.Default__?.Url ?? string.Empty,
                SubscriberCount: (long)(channel.Statistics?.SubscriberCount ?? 0)
            ));

            return Result.Success(results);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<YoutubeChannelSearchResult>>(
                new Error("Youtube.SearchChannelsFailed", ex.Message)
            );
        }
    }

    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> GetChannelVideosAsync(
        string channelId,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            // Get the uploads playlist ID for the channel
            var channelRequest = _youtubeService.Channels.List("contentDetails");
            channelRequest.Id = channelId;

            var channelResponse = await channelRequest.ExecuteAsync(cancellationToken);

            if (channelResponse.Items.Count == 0)
            {
                return Result.Failure<IEnumerable<YoutubeVideoPreview>>(
                    YoutubeChannelsErrors.NotFoundByYoutubeId(channelId)
                );
            }

            var uploadsPlaylistId = channelResponse
                .Items[0]
                .ContentDetails
                .RelatedPlaylists
                .Uploads;

            // Fetch videos from uploads playlist with pagination, filtering out Shorts
            var result = new List<YoutubeVideoPreview>();
            string? nextPageToken = null;

            while (result.Count < maxResults)
            {
                // Read playlist items (up to 50 per request)
                var playlistRequest = _youtubeService.PlaylistItems.List("contentDetails");
                playlistRequest.PlaylistId = uploadsPlaylistId;
                playlistRequest.MaxResults = 50;
                playlistRequest.PageToken = nextPageToken;

                var playlistResponse = await playlistRequest.ExecuteAsync(cancellationToken);

                var videoIds = playlistResponse
                    .Items.Select(i => i.ContentDetails.VideoId)
                    .Where(id => !string.IsNullOrEmpty(id))
                    .ToList();

                if (videoIds.Count == 0)
                {
                    break;
                }

                // Get video details (duration, view count)
                var videosRequest = _youtubeService.Videos.List(
                    "snippet,contentDetails,statistics"
                );
                videosRequest.Id = string.Join(",", videoIds);

                var videosResponse = await videosRequest.ExecuteAsync(cancellationToken);

                foreach (var video in videosResponse.Items)
                {
                    // Filter out YouTube Shorts (videos <= 62 seconds)
                    var durationSeconds = GetDurationInSeconds(video.ContentDetails?.Duration);

                    if (durationSeconds > 180)
                    {
                        result.Add(
                            new YoutubeVideoPreview(
                                VideoId: video.Id,
                                Title: video.Snippet.Title,
                                ThumbnailUrl: video.Snippet.Thumbnails?.Medium?.Url
                                    ?? video.Snippet.Thumbnails?.Default__?.Url
                                    ?? string.Empty,
                                ChannelName: video.Snippet.ChannelTitle,
                                ChannelId: video.Snippet.ChannelId,
                                PublishedAt: video.Snippet.PublishedAtDateTimeOffset?.DateTime
                                    ?? DateTime.MinValue,
                                Duration: FormatDuration(video.ContentDetails?.Duration),
                                ViewCount: (long)(video.Statistics?.ViewCount ?? 0)
                            )
                        );

                        if (result.Count >= maxResults)
                        {
                            break;
                        }
                    }
                }

                nextPageToken = playlistResponse.NextPageToken;
                if (string.IsNullOrEmpty(nextPageToken))
                {
                    break;
                }
            }

            return Result.Success<IEnumerable<YoutubeVideoPreview>>(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<YoutubeVideoPreview>>(
                new Error("Youtube.GetChannelVideosFailed", ex.Message)
            );
        }
    }

    private static int GetDurationInSeconds(string? isoDuration)
    {
        if (string.IsNullOrEmpty(isoDuration))
        {
            return 0;
        }

        try
        {
            return (int)XmlConvert.ToTimeSpan(isoDuration).TotalSeconds;
        }
        catch
        {
            return 0;
        }
    }

    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> GetVideosFromChannelsAsync(
        IEnumerable<string> channelIds,
        int maxResultsPerChannel = 5,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var allVideos = new List<YoutubeVideoPreview>();

            // Fetch videos from each channel in parallel
            var tasks = channelIds.Select(channelId =>
                GetChannelVideosAsync(channelId, maxResultsPerChannel, cancellationToken)
            );

            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (result.IsSuccess)
                {
                    allVideos.AddRange(result.Value);
                }
            }

            // Sort by publish date descending
            var sortedVideos = allVideos.OrderByDescending(v => v.PublishedAt).ToList();

            return Result.Success<IEnumerable<YoutubeVideoPreview>>(sortedVideos);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<YoutubeVideoPreview>>(
                new Error("Youtube.GetVideosFromChannelsFailed", ex.Message)
            );
        }
    }

    public async Task<Result<IEnumerable<YoutubeVideoPreview>>> SearchVideosAsync(
        string query,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var searchRequest = _youtubeService.Search.List("snippet");
            searchRequest.Q = query;
            searchRequest.Type = "video";
            searchRequest.MaxResults = maxResults;

            var searchResponse = await searchRequest.ExecuteAsync(cancellationToken);

            var videoIds = searchResponse
                .Items.Select(item => item.Id.VideoId)
                .Where(id => !string.IsNullOrEmpty(id))
                .ToList();

            if (videoIds.Count == 0)
            {
                return Result.Success(Enumerable.Empty<YoutubeVideoPreview>());
            }

            // Get video details
            var videosRequest = _youtubeService.Videos.List("snippet,contentDetails,statistics");
            videosRequest.Id = string.Join(",", videoIds);

            var videosResponse = await videosRequest.ExecuteAsync(cancellationToken);

            var results = videosResponse.Items.Select(video => new YoutubeVideoPreview(
                VideoId: video.Id,
                Title: video.Snippet.Title,
                ThumbnailUrl: video.Snippet.Thumbnails?.Medium?.Url
                    ?? video.Snippet.Thumbnails?.Default__?.Url
                    ?? string.Empty,
                ChannelName: video.Snippet.ChannelTitle,
                ChannelId: video.Snippet.ChannelId,
                PublishedAt: video.Snippet.PublishedAtDateTimeOffset?.DateTime ?? DateTime.MinValue,
                Duration: FormatDuration(video.ContentDetails?.Duration),
                ViewCount: (long)(video.Statistics?.ViewCount ?? 0)
            ));

            return Result.Success(results);
        }
        catch (Exception ex)
        {
            return Result.Failure<IEnumerable<YoutubeVideoPreview>>(
                new Error("Youtube.SearchVideosFailed", ex.Message)
            );
        }
    }

    public async Task<Result<YoutubeVideoDetails>> GetVideoDetailsAsync(
        string videoId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var videosRequest = _youtubeService.Videos.List("snippet,contentDetails,statistics");
            videosRequest.Id = videoId;

            var videosResponse = await videosRequest.ExecuteAsync(cancellationToken);

            var video = videosResponse.Items.FirstOrDefault();

            if (video is null)
            {
                return Result.Failure<YoutubeVideoDetails>(
                    new Error(
                        "Youtube.VideoNotFound",
                        $"Video with ID '{videoId}' was not found.",
                        404
                    )
                );
            }

            var details = new YoutubeVideoDetails(
                VideoId: video.Id,
                Title: video.Snippet.Title,
                Description: video.Snippet.Description ?? string.Empty,
                EmbedUrl: $"https://www.youtube.com/embed/{video.Id}",
                ThumbnailUrl: video.Snippet.Thumbnails?.Maxres?.Url
                    ?? video.Snippet.Thumbnails?.High?.Url
                    ?? video.Snippet.Thumbnails?.Medium?.Url
                    ?? string.Empty,
                ChannelName: video.Snippet.ChannelTitle,
                ChannelId: video.Snippet.ChannelId,
                PublishedAt: video.Snippet.PublishedAtDateTimeOffset?.DateTime ?? DateTime.MinValue,
                Duration: FormatDuration(video.ContentDetails?.Duration),
                ViewCount: (long)(video.Statistics?.ViewCount ?? 0),
                LikeCount: (long)(video.Statistics?.LikeCount ?? 0)
            );

            return Result.Success(details);
        }
        catch (Exception ex)
        {
            return Result.Failure<YoutubeVideoDetails>(
                new Error("Youtube.GetVideoDetailsFailed", ex.Message)
            );
        }
    }

    public async Task<Result<YoutubeChannelSearchResult>> GetChannelInfoAsync(
        string channelId,
        CancellationToken cancellationToken = default
    )
    {
        try
        {
            var channelsRequest = _youtubeService.Channels.List("snippet,statistics");
            channelsRequest.Id = channelId;

            var channelsResponse = await channelsRequest.ExecuteAsync(cancellationToken);

            var channel = channelsResponse.Items.FirstOrDefault();

            if (channel is null)
            {
                return Result.Failure<YoutubeChannelSearchResult>(
                    new Error(
                        "Youtube.ChannelNotFound",
                        $"Channel with ID '{channelId}' was not found.",
                        404
                    )
                );
            }

            var result = new YoutubeChannelSearchResult(
                ChannelId: channel.Id,
                Name: channel.Snippet.Title,
                Description: channel.Snippet.Description ?? string.Empty,
                ThumbnailUrl: channel.Snippet.Thumbnails?.Default__?.Url ?? string.Empty,
                SubscriberCount: (long)(channel.Statistics?.SubscriberCount ?? 0)
            );

            return Result.Success(result);
        }
        catch (Exception ex)
        {
            return Result.Failure<YoutubeChannelSearchResult>(
                new Error("Youtube.GetChannelInfoFailed", ex.Message)
            );
        }
    }

    private static string FormatDuration(string? isoDuration)
    {
        if (string.IsNullOrEmpty(isoDuration))
        {
            return "0:00";
        }

        try
        {
            var duration = XmlConvert.ToTimeSpan(isoDuration);

            if (duration.TotalHours >= 1)
            {
                return $"{(int)duration.TotalHours}:{duration.Minutes:D2}:{duration.Seconds:D2}";
            }

            return $"{duration.Minutes}:{duration.Seconds:D2}";
        }
        catch
        {
            return "0:00";
        }
    }

    public void Dispose()
    {
        _youtubeService?.Dispose();
    }
}
