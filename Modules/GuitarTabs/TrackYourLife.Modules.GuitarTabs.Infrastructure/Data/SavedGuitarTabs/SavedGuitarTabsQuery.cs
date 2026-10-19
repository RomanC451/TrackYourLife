using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.SavedGuitarTabs;

internal sealed class SavedGuitarTabsQuery(GuitarTabsReadDbContext dbContext) : ISavedGuitarTabsQuery
{
    public async Task<SavedGuitarTabReadModel?> GetByUserAndUrlAsync(
        UserId userId,
        string tabUrl,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.SavedGuitarTabs.FirstOrDefaultAsync(
            tab => tab.UserId == userId && tab.TabUrl == tabUrl,
            cancellationToken
        );
    }

    public async Task<IReadOnlyList<GuitarPlaylistId>> GetPlaylistIdsAsync(
        SavedGuitarTabId savedGuitarTabId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .GuitarPlaylistSongs.Where(song => song.SavedGuitarTabId == savedGuitarTabId)
            .Select(song => song.GuitarPlaylistId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SavedGuitarTabReadModel>> GetFavoritesAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .SavedGuitarTabs.Where(tab => tab.UserId == userId && tab.IsFavorite)
            .OrderBy(tab => tab.ArtistName)
            .ThenBy(tab => tab.SongName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SavedGuitarTabReadModel>> GetByPlaylistAsync(
        UserId userId,
        GuitarPlaylistId playlistId,
        CancellationToken cancellationToken = default
    )
    {
        return await (
            from membership in dbContext.GuitarPlaylistSongs
            join tab in dbContext.SavedGuitarTabs on membership.SavedGuitarTabId equals tab.Id
            where membership.GuitarPlaylistId == playlistId && tab.UserId == userId
            orderby membership.AddedOnUtc
            select tab
        ).ToListAsync(cancellationToken);
    }
}
