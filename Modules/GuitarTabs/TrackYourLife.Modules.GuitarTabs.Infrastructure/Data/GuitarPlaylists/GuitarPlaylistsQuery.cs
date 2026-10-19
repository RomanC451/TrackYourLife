using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.GuitarPlaylists;

internal sealed class GuitarPlaylistsQuery(GuitarTabsReadDbContext dbContext) : IGuitarPlaylistsQuery
{
    public async Task<IReadOnlyList<GuitarPlaylistReadModel>> GetByUserIdAsync(
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .GuitarPlaylists.AsNoTracking()
            .Where(playlist => playlist.UserId == userId)
            .OrderBy(playlist => playlist.CreatedOnUtc)
            .ThenBy(playlist => playlist.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<GuitarPlaylistReadModel?> GetByIdAndUserIdAsync(
        GuitarPlaylistId id,
        UserId userId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .GuitarPlaylists.AsNoTracking()
            .FirstOrDefaultAsync(
                playlist => playlist.Id == id && playlist.UserId == userId,
                cancellationToken
            );
    }
}
