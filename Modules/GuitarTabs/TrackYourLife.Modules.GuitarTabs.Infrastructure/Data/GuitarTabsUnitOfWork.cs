using TrackYourLife.Modules.GuitarTabs.Domain.Core;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.GuitarTabs.Infrastructure.Data;

internal sealed class GuitarTabsUnitOfWork(GuitarTabsWriteDbContext dbContext)
    : UnitOfWork<GuitarTabsWriteDbContext>(dbContext),
        IGuitarTabsUnitOfWork;
