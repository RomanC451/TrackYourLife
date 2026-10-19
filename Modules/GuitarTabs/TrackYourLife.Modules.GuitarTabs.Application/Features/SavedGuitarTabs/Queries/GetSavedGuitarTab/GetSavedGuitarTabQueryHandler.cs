using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetSavedGuitarTab;

internal sealed class GetSavedGuitarTabQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    ISavedGuitarTabsQuery savedGuitarTabsQuery
) : IQueryHandler<GetSavedGuitarTabQuery, GuitarTabLibraryStateResponse>
{
    public async Task<Result<GuitarTabLibraryStateResponse>> Handle(
        GetSavedGuitarTabQuery request,
        CancellationToken cancellationToken
    )
    {
        if (!UltimateGuitarTabUrl.TryCreate(request.Url, out var tabUrl))
        {
            return Result.Success(new GuitarTabLibraryStateResponse(false, [], null));
        }

        var tab = await savedGuitarTabsQuery.GetByUserAndUrlAsync(
            userIdentifierProvider.UserId,
            tabUrl.AbsoluteUri,
            cancellationToken
        );
        if (tab is null)
        {
            return Result.Success(new GuitarTabLibraryStateResponse(false, [], null));
        }

        var playlistIds = await savedGuitarTabsQuery.GetPlaylistIdsAsync(tab.Id, cancellationToken);
        return Result.Success(
            new GuitarTabLibraryStateResponse(
                tab.IsFavorite,
                playlistIds.Select(id => id.Value).ToArray(),
                tab.YoutubeUrl
            )
        );
    }
}
