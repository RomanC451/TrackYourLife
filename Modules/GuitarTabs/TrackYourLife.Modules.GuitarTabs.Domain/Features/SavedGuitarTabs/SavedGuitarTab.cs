using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

public sealed class SavedGuitarTab : Entity<SavedGuitarTabId>, IAuditableEntity
{
    public const int SongNameMaxLength = 300;
    public const int ArtistNameMaxLength = 300;
    public const int TypeMaxLength = 80;
    public const int TabUrlMaxLength = 500;
    public const int YoutubeUrlMaxLength = 100;

    public UserId UserId { get; private set; } = UserId.Empty;
    public string TabUrl { get; private set; } = string.Empty;
    public string SongName { get; private set; } = string.Empty;
    public string ArtistName { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public bool IsFavorite { get; private set; }
    public string? YoutubeUrl { get; private set; }
    public DateTime CreatedOnUtc { get; private set; }
    public DateTime? ModifiedOnUtc { get; private set; }

    private SavedGuitarTab()
        : base() { }

    private SavedGuitarTab(
        SavedGuitarTabId id,
        UserId userId,
        string tabUrl,
        string songName,
        string artistName,
        string type,
        bool isFavorite,
        DateTime createdOnUtc
    )
        : base(id)
    {
        UserId = userId;
        TabUrl = tabUrl;
        SongName = songName;
        ArtistName = artistName;
        Type = type;
        IsFavorite = isFavorite;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<SavedGuitarTab> Create(
        SavedGuitarTabId id,
        UserId userId,
        string tabUrl,
        string songName,
        string artistName,
        string type,
        bool isFavorite,
        DateTime createdOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(SavedGuitarTab), nameof(id))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(SavedGuitarTab), nameof(userId))
            ),
            Ensure.NotEmpty(
                tabUrl,
                DomainErrors.ArgumentError.Empty(nameof(SavedGuitarTab), nameof(tabUrl))
            ),
            Ensure.NotEmpty(
                songName,
                DomainErrors.ArgumentError.Empty(nameof(SavedGuitarTab), nameof(songName))
            ),
            Ensure.NotEmpty(
                artistName,
                DomainErrors.ArgumentError.Empty(nameof(SavedGuitarTab), nameof(artistName))
            ),
            Ensure.NotEmpty(
                type,
                DomainErrors.ArgumentError.Empty(nameof(SavedGuitarTab), nameof(type))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(SavedGuitarTab), nameof(createdOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<SavedGuitarTab>(result.Error);
        }

        if (
            songName.Trim().Length > SongNameMaxLength
            || artistName.Trim().Length > ArtistNameMaxLength
            || type.Trim().Length > TypeMaxLength
            || tabUrl.Length > TabUrlMaxLength
        )
        {
            return Result.Failure<SavedGuitarTab>(
                DomainErrors.ArgumentError.Invalid(nameof(SavedGuitarTab), nameof(songName))
            );
        }

        return Result.Success(
            new SavedGuitarTab(
                id,
                userId,
                tabUrl,
                songName.Trim(),
                artistName.Trim(),
                type.Trim(),
                isFavorite,
                createdOnUtc
            )
        );
    }

    public void UpdateDetails(string songName, string artistName, string type, DateTime modifiedOnUtc)
    {
        SongName = songName.Trim();
        ArtistName = artistName.Trim();
        Type = type.Trim();
        ModifiedOnUtc = modifiedOnUtc;
    }

    public void MarkFavorite(DateTime modifiedOnUtc)
    {
        IsFavorite = true;
        ModifiedOnUtc = modifiedOnUtc;
    }

    public void ClearFavorite(DateTime modifiedOnUtc)
    {
        IsFavorite = false;
        ModifiedOnUtc = modifiedOnUtc;
    }

    public Result SetYoutubeUrl(string? youtubeUrl, DateTime modifiedOnUtc)
    {
        if (string.IsNullOrWhiteSpace(youtubeUrl))
        {
            YoutubeUrl = null;
            ModifiedOnUtc = modifiedOnUtc;
            return Result.Success();
        }

        if (!YoutubeVideoUrl.TryNormalize(youtubeUrl, out var normalized))
        {
            return Result.Failure(SavedGuitarTabErrors.InvalidYoutubeUrl);
        }

        YoutubeUrl = normalized;
        ModifiedOnUtc = modifiedOnUtc;
        return Result.Success();
    }
}
