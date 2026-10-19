using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data.SavedGuitarTabs;

internal sealed class SavedGuitarTabsRepository(GuitarTabsWriteDbContext dbContext)
    : GenericRepository<SavedGuitarTab, SavedGuitarTabId>(dbContext.SavedGuitarTabs),
        ISavedGuitarTabsRepository
{
    public async Task<SavedGuitarTab?> GetByUserAndUrlAsync(
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
}
