using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public sealed class YoutubePlaylistVideo : Entity<YoutubePlaylistVideoId>
{
    public YoutubePlaylistId YoutubePlaylistId { get; private set; } = YoutubePlaylistId.Empty;
    public string YoutubeId { get; private set; } = string.Empty;
    public DateTime AddedOnUtc { get; private set; }

    private YoutubePlaylistVideo()
        : base() { }

    private YoutubePlaylistVideo(
        YoutubePlaylistVideoId id,
        YoutubePlaylistId youtubePlaylistId,
        string youtubeId,
        DateTime addedOnUtc
    )
        : base(id)
    {
        YoutubePlaylistId = youtubePlaylistId;
        YoutubeId = youtubeId;
        AddedOnUtc = addedOnUtc;
    }

    public static Result<YoutubePlaylistVideo> Create(
        YoutubePlaylistVideoId id,
        YoutubePlaylistId youtubePlaylistId,
        string videoId,
        DateTime addedOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylistVideo), nameof(id))
            ),
            Ensure.NotEmptyId(
                youtubePlaylistId,
                DomainErrors.ArgumentError.Empty(
                    nameof(YoutubePlaylistVideo),
                    nameof(youtubePlaylistId)
                )
            ),
            Ensure.NotEmpty(
                videoId,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylistVideo), nameof(videoId))
            ),
            Ensure.NotEmpty(
                addedOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylistVideo), nameof(addedOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<YoutubePlaylistVideo>(result.Error);
        }

        return Result.Success(
            new YoutubePlaylistVideo(id, youtubePlaylistId, videoId.Trim(), addedOnUtc)
        );
    }
}
