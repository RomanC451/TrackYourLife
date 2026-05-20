using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;

public sealed class YoutubeCategory : Entity<YoutubeCategoryId>, IAuditableEntity
{
    public const int MaxNameLength = 120;

    public UserId UserId { get; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public int MaxVideosPerDay { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; }

    private YoutubeCategory()
        : base() { }

    private YoutubeCategory(
        YoutubeCategoryId id,
        UserId userId,
        string name,
        int maxVideosPerDay,
        int displayOrder,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        MaxVideosPerDay = maxVideosPerDay;
        DisplayOrder = displayOrder;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<YoutubeCategory> Create(
        YoutubeCategoryId id,
        UserId userId,
        string name,
        int maxVideosPerDay,
        int displayOrder,
        DateTime createdOnUtc
    )
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
        {
            return Result.Failure<YoutubeCategory>(nameResult.Error);
        }

        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeCategory), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeCategory), nameof(userId))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(YoutubeCategory), nameof(createdOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<YoutubeCategory>(result.Error);
        }

        if (maxVideosPerDay < 0)
        {
            return Result.Failure<YoutubeCategory>(
                new Error(
                    "Youtube.Category.InvalidMaxVideos",
                    "Max videos per day cannot be negative.",
                    400
                )
            );
        }

        return Result.Success(
            new YoutubeCategory(id, userId, name.Trim(), maxVideosPerDay, displayOrder, createdOnUtc)
        );
    }

    public Result UpdateName(string name, DateTime utcNow)
    {
        var nameResult = ValidateName(name);
        if (nameResult.IsFailure)
        {
            return nameResult;
        }

        Name = name.Trim();
        ModifiedOnUtc = utcNow;
        return Result.Success();
    }

    public Result UpdateMaxVideosPerDay(int maxVideosPerDay, DateTime utcNow)
    {
        if (maxVideosPerDay < 0)
        {
            return Result.Failure(
                new Error(
                    "Youtube.Category.InvalidMaxVideos",
                    "Max videos per day cannot be negative.",
                    400
                )
            );
        }

        MaxVideosPerDay = maxVideosPerDay;
        ModifiedOnUtc = utcNow;
        return Result.Success();
    }

    public void UpdateDisplayOrder(int displayOrder, DateTime utcNow)
    {
        DisplayOrder = displayOrder;
        ModifiedOnUtc = utcNow;
    }

    private static Result ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(YoutubeCategoriesErrors.NameRequired);
        }

        if (name.Trim().Length > MaxNameLength)
        {
            return Result.Failure(YoutubeCategoriesErrors.NameTooLong);
        }

        return Result.Success();
    }
}
