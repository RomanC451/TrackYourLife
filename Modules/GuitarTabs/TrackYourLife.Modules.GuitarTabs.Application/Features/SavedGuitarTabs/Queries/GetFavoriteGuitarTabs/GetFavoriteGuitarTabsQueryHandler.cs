using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetFavoriteGuitarTabs;

internal sealed class GetFavoriteGuitarTabsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ISavedGuitarTabsQuery savedGuitarTabsQuery
) : IQueryHandler<GetFavoriteGuitarTabsQuery, IReadOnlyList<SavedGuitarTabResponse>>
{
    public async Task<Result<IReadOnlyList<SavedGuitarTabResponse>>> Handle(
        GetFavoriteGuitarTabsQuery request,
        CancellationToken cancellationToken
    )
    {
        var tabs = await savedGuitarTabsQuery.GetFavoritesAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );
        return Result.Success<IReadOnlyList<SavedGuitarTabResponse>>(tabs.Select(SavedGuitarTabMapper.Map).ToArray());
    }
}
