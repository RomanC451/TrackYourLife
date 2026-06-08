using TrackYourLife.Modules.Youtube.Application.Features.YoutubeVideos.Models;
using TrackYourLife.Modules.Youtube.Application.Services;
using TrackYourLife.Modules.Youtube.Contracts.Dtos;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.App.E2e;

internal sealed class E2EYoutubeApiService : IYoutubeApiService
{
    public Task<Result<IEnumerable<YoutubeChannelSearchResult>>> SearchChannelsAsync(
        string query,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(Result.Success(Enumerable.Empty<YoutubeChannelSearchResult>()));

    public Task<Result<IEnumerable<YoutubeVideoPreview>>> GetChannelVideosAsync(
        string channelId,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(Result.Success(Enumerable.Empty<YoutubeVideoPreview>()));

    public Task<Result<IEnumerable<YoutubeVideoPreview>>> GetVideosFromChannelsAsync(
        IEnumerable<string> channelIds,
        int maxResultsPerChannel = 5,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(Result.Success(Enumerable.Empty<YoutubeVideoPreview>()));

    public Task<Result<IEnumerable<YoutubeVideoPreview>>> SearchVideosAsync(
        string query,
        int maxResults = 10,
        CancellationToken cancellationToken = default
    ) => Task.FromResult(Result.Success(Enumerable.Empty<YoutubeVideoPreview>()));

    public Task<Result<IReadOnlyDictionary<string, YoutubeVideoPreview>>> GetVideoPreviewsByIdsAsync(
        IEnumerable<string> videoIds,
        CancellationToken cancellationToken = default
    ) =>
        Task.FromResult(
            Result.Success<IReadOnlyDictionary<string, YoutubeVideoPreview>>(
                new Dictionary<string, YoutubeVideoPreview>()
            )
        );

    public Task<Result<YoutubeVideoDetails>> GetVideoDetailsAsync(
        string videoId,
        CancellationToken cancellationToken = default
    ) =>
        Task.FromResult(
            Result.Success(
                new YoutubeVideoDetails(
                    videoId,
                    "E2E Video",
                    "E2E description",
                    $"https://piped.example/embed/{videoId}",
                    "https://example.com/thumb.jpg",
                    "E2E Channel",
                    "channel_e2e",
                    DateTime.UtcNow.AddDays(-1),
                    "PT5M",
                    100,
                    10
                )
            )
        );

    public Task<Result<YoutubeChannelSearchResult>> GetChannelInfoAsync(
        string channelId,
        CancellationToken cancellationToken = default
    ) =>
        Task.FromResult(
            Result.Success(
                new YoutubeChannelSearchResult(
                    channelId,
                    "E2E Channel",
                    "E2E channel description",
                    "https://example.com/channel.jpg",
                    1000,
                    false
                )
            )
        );
}
