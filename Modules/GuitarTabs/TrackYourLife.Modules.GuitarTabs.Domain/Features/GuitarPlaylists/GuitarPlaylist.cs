using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

public sealed class GuitarPlaylist : Entity<GuitarPlaylistId>, IAuditableEntity
{
    public const int NameMaxLength = 200;

    public UserId UserId { get; } = UserId.Empty;
    public string Name { get; private set; } = string.Empty;
    public DateTime CreatedOnUtc { get; }
    public DateTime? ModifiedOnUtc { get; private set; }

    private GuitarPlaylist()
        : base() { }

    private GuitarPlaylist(
        GuitarPlaylistId id,
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

    public static Result<GuitarPlaylist> Create(
        GuitarPlaylistId id,
        UserId userId,
        string name,
        DateTime createdOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylist), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylist), nameof(userId))
            ),
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylist), nameof(name))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylist), nameof(createdOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<GuitarPlaylist>(result.Error);
        }

        var trimmedName = name.Trim();
        if (IsReservedName(trimmedName))
        {
            return Result.Failure<GuitarPlaylist>(GuitarPlaylistErrors.NameReserved);
        }

        if (trimmedName.Length > NameMaxLength)
        {
            return Result.Failure<GuitarPlaylist>(
                DomainErrors.ArgumentError.Invalid(nameof(GuitarPlaylist), nameof(name))
            );
        }

        return Result.Success(new GuitarPlaylist(id, userId, trimmedName, createdOnUtc));
    }

    public Result UpdateName(string name, DateTime modifiedOnUtc)
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(
                name,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylist), nameof(name))
            ),
            Ensure.NotEmpty(
                modifiedOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylist), nameof(modifiedOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return result;
        }

        var trimmedName = name.Trim();
        if (IsReservedName(trimmedName))
        {
            return Result.Failure(GuitarPlaylistErrors.NameReserved);
        }

        if (trimmedName.Length > NameMaxLength)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Invalid(nameof(GuitarPlaylist), nameof(name))
            );
        }

        Name = trimmedName;
        ModifiedOnUtc = modifiedOnUtc;
        return Result.Success();
    }

    private static bool IsReservedName(string name) =>
        name.Equals(GuitarPlaylistErrors.FavoritesName, StringComparison.OrdinalIgnoreCase);
}
