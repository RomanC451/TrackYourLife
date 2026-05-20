using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
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
    public YoutubeCategoryId YoutubeCategoryId { get; private set; } = YoutubeCategoryId.Empty;
    public string CategoryName { get; private set; } = string.Empty;
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; }

    private YoutubeChannel()
        : base() { }

    private YoutubeChannel(
        YoutubeChannelId id,
        UserId userId,
        string youtubeChannelId,
        string name,
        string? thumbnailUrl,
        YoutubeCategoryId youtubeCategoryId,
        string categoryName,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        YoutubeChannelId = youtubeChannelId;
        Name = name;
        ThumbnailUrl = thumbnailUrl;
        YoutubeCategoryId = youtubeCategoryId;
        CategoryName = categoryName;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<YoutubeChannel> Create(
        YoutubeChannelId id,
        UserId userId,
        string youtubeChannelId,
        string name,
        string? thumbnailUrl,
        YoutubeCategoryId youtubeCategoryId,
        string categoryName,
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
            ),
            Ensure.NotEmptyId(
                youtubeCategoryId,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(youtubeCategoryId))
            ),
            Ensure.NotEmpty(
                categoryName,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(categoryName))
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
                youtubeCategoryId,
                categoryName.Trim(),
                createdOnUtc
            )
        );
    }

    public Result AssignCategory(YoutubeCategoryId youtubeCategoryId, string categoryName, DateTime utcNow)
    {
        var check = Ensure.NotEmptyId(
            youtubeCategoryId,
            DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(youtubeCategoryId))
        );
        if (check.IsFailure)
        {
            return check;
        }

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Empty(nameof(YoutubeChannel), nameof(categoryName))
            );
        }

        YoutubeCategoryId = youtubeCategoryId;
        CategoryName = categoryName.Trim();
        ModifiedOnUtc = utcNow;
        return Result.Success();
    }

    public void SyncCategoryName(string categoryName, DateTime utcNow)
    {
        CategoryName = categoryName.Trim();
        ModifiedOnUtc = utcNow;
    }
}
