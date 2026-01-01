using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.WatchedVideos;

public sealed class WatchedVideo : Entity<WatchedVideoId>
{
    public UserId UserId { get; } = UserId.Empty;
    public string VideoId { get; private set; } = string.Empty;
    public string ChannelId { get; private set; } = string.Empty;
    public DateTime WatchedAtUtc { get; private set; }

    private WatchedVideo()
        : base() { }

    private WatchedVideo(
        WatchedVideoId id,
        UserId userId,
        string videoId,
        string channelId,
        DateTime watchedAtUtc
    )
        : base(id)
    {
        UserId = userId;
        VideoId = videoId;
        ChannelId = channelId;
        WatchedAtUtc = watchedAtUtc;
    }

    public static Result<WatchedVideo> Create(
        WatchedVideoId id,
        UserId userId,
        string videoId,
        string channelId,
        DateTime watchedAtUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(WatchedVideo), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(WatchedVideo), nameof(userId))
            ),
            Ensure.NotEmpty(
                videoId,
                DomainErrors.ArgumentError.Empty(nameof(WatchedVideo), nameof(videoId))
            ),
            Ensure.NotEmpty(
                channelId,
                DomainErrors.ArgumentError.Empty(nameof(WatchedVideo), nameof(channelId))
            ),
            Ensure.NotEmpty(
                watchedAtUtc,
                DomainErrors.ArgumentError.Empty(nameof(WatchedVideo), nameof(watchedAtUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<WatchedVideo>(result.Error);
        }

        return Result.Success(
            new WatchedVideo(id, userId, videoId, channelId, watchedAtUtc)
        );
    }
}
