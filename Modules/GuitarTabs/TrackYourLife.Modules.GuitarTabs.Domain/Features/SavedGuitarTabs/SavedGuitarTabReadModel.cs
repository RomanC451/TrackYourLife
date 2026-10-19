using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

public sealed record SavedGuitarTabReadModel(
    SavedGuitarTabId Id,
    UserId UserId,
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type,
    bool IsFavorite,
    string? YoutubeUrl,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<SavedGuitarTabId>;
