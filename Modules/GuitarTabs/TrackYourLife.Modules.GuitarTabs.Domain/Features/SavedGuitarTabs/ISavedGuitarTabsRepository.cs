using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

public interface ISavedGuitarTabsRepository
{
    Task<SavedGuitarTab?> GetByIdAsync(
        SavedGuitarTabId id,
        CancellationToken cancellationToken = default
    );

    Task<SavedGuitarTab?> GetByUserAndUrlAsync(
        UserId userId,
        string tabUrl,
        CancellationToken cancellationToken = default
    );

    Task AddAsync(SavedGuitarTab tab, CancellationToken cancellationToken = default);

    void Remove(SavedGuitarTab tab);

    void Update(SavedGuitarTab tab);
}
