using TrackYourLife.Modules.Youtube.Application.Features.YoutubeChannels.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Youtube.Application.Services;

public interface IYoutubeApiService
{
    /// <summary>
    /// Searches for YouTube channels by name.
    /// </summary>
    Task<Result<IEnumerable<YoutubeChannelSearchResult>>> SearchChannelsAsync(
        string query,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the latest videos from a specific channel.
    /// </summary>
    Task<Result<IEnumerable<YoutubeVideoPreview>>> GetChannelVideosAsync(
        string channelId,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets the latest videos from multiple channels.
    /// </summary>
    Task<Result<IEnumerable<YoutubeVideoPreview>>> GetVideosFromChannelsAsync(
        IEnumerable<string> channelIds,
        int maxResultsPerChannel = 5,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Searches for YouTube videos by name.
    /// </summary>
    Task<Result<IEnumerable<YoutubeVideoPreview>>> SearchVideosAsync(
        string query,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets detailed information about a specific video.
    /// </summary>
    Task<Result<YoutubeVideoDetails>> GetVideoDetailsAsync(
        string videoId,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Gets channel information by YouTube channel ID.
    /// </summary>
    Task<Result<YoutubeChannelSearchResult>> GetChannelInfoAsync(
        string channelId,
        CancellationToken cancellationToken = default
    );
}

