using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetPlaylistSongs;

internal sealed class GetPlaylistSongsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarPlaylistsQuery guitarPlaylistsQuery,
    ISavedGuitarTabsQuery savedGuitarTabsQuery
) : IQueryHandler<GetPlaylistSongsQuery, IReadOnlyList<SavedGuitarTabResponse>>
{
    public async Task<Result<IReadOnlyList<SavedGuitarTabResponse>>> Handle(
        GetPlaylistSongsQuery request,
        CancellationToken cancellationToken
    )
    {
        var playlist = await guitarPlaylistsQuery.GetByIdAndUserIdAsync(
            request.GuitarPlaylistId,
            userIdentifierProvider.UserId,
            cancellationToken
        );
        if (playlist is null)
        {
            return Result.Failure<IReadOnlyList<SavedGuitarTabResponse>>(
                GuitarPlaylistErrors.NotFound(request.GuitarPlaylistId.Value)
            );
        }

        var tabs = await savedGuitarTabsQuery.GetByPlaylistAsync(
            userIdentifierProvider.UserId,
            request.GuitarPlaylistId,
            cancellationToken
        );
        return Result.Success<IReadOnlyList<SavedGuitarTabResponse>>(tabs.Select(SavedGuitarTabMapper.Map).ToArray());
    }
}