using TrackYourLife.Modules.Youtube.Domain.Core;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeChannels;

public sealed class YoutubeChannel : Entity<YoutubeChannelId>, IAuditableEntity
{
    public UserId UserId { get; } = UserId.Empty;
    public string YoutubeChannelId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string? ThumbnailUrl { get; private set; }
    public VideoCategory Category { get; private set; }
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; }

    private YoutubeChannel()
        : base() { }

    private YoutubeChannel(
        YoutubeChannelId id,
        UserId userId,
        string youtubeChannelId,
        string name,
        string? thumbnailUrl,
        VideoCategory category,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        YoutubeChannelId = youtubeChannelId;
        Name = name;
        ThumbnailUrl = thumbnailUrl;
        Category = category;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<YoutubeChannel> Create(
        YoutubeChannelId id,
        UserId userId,
        string youtubeChannelId,
        string name,
        string? thumbnailUrl,
        VideoCategory category,
        DateTime createdOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(userId))
            ),
            Ensure.NotEmpty(
                youtubeChannelId,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(youtubeChannelId))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(name))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(createdOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<YoutubeChannel>(result.Error);
        }

        return Result.Success(
            new YoutubeChannel(
                id,
                userId,
                youtubeChannelId,
                name,
                thumbnailUrl,
                category,
                createdOnUtc
            )
        );
    }

    public Result UpdateCategory(VideoCategory category)
    {
        Category = category;
        return Result.Success();
    }
}

