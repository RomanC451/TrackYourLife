using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylistSongs;

public sealed class GuitarPlaylistSong : Entity<GuitarPlaylistSongId>
{
    public GuitarPlaylistId GuitarPlaylistId { get; private set; } = GuitarPlaylistId.Empty;
    public SavedGuitarTabId SavedGuitarTabId { get; private set; } = SavedGuitarTabId.Empty;
    public DateTime AddedOnUtc { get; private set; }

    private GuitarPlaylistSong()
        : base() { }

    private GuitarPlaylistSong(
        GuitarPlaylistSongId id,
        GuitarPlaylistId guitarPlaylistId,
        SavedGuitarTabId savedGuitarTabId,
        DateTime addedOnUtc
    )
        : base(id)
    {
        GuitarPlaylistId = guitarPlaylistId;
        SavedGuitarTabId = savedGuitarTabId;
        AddedOnUtc = addedOnUtc;
    }

    public static Result<GuitarPlaylistSong> Create(
        GuitarPlaylistSongId id,
        GuitarPlaylistId guitarPlaylistId,
        SavedGuitarTabId savedGuitarTabId,
        DateTime addedOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylistSong), nameof(id))
            ),
            Ensure.NotEmptyId(
                guitarPlaylistId,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylistSong), nameof(guitarPlaylistId))
            ),
            Ensure.NotEmptyId(
                savedGuitarTabId,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylistSong), nameof(savedGuitarTabId))
            ),
            Ensure.NotEmpty(
                addedOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(GuitarPlaylistSong), nameof(addedOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<GuitarPlaylistSong>(result.Error);
        }

        return Result.Success(
            new GuitarPlaylistSong(id, guitarPlaylistId, savedGuitarTabId, addedOnUtc)
        );
    }
}
