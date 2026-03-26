using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

public sealed class YoutubePlaylist : Entity<YoutubePlaylistId>, IAuditableEntity
{
    public UserId UserId { get; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; }

    private YoutubePlaylist()
        : base() { }

    private YoutubePlaylist(
        YoutubePlaylistId id,
        UserId userId,
        string name,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        Name = name;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<YoutubePlaylist> Create(
        YoutubePlaylistId id,
        UserId userId,
        string name,
        DateTime createdOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylist), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylist), nameof(userId))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylist), nameof(name))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylist), nameof(createdOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<YoutubePlaylist>(result.Error);
        }

        return Result.Success(new YoutubePlaylist(id, userId, name.Trim(), createdOnUtc));
    }

    public Result UpdateName(string name, DateTime modifiedOnUtc)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylist), nameof(name))
            ),
            Ensure.NotEmpty(
                modifiedOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(YoutubePlaylist), nameof(modifiedOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return result;
        }

        Name = name.Trim();
        ModifiedOnUtc = modifiedOnUtc;
        return Result.Success();
    }
}
