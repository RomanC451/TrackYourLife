using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Models;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Queries.GetPlaylists;

internal sealed class GetPlaylistsQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarPlaylistsQuery guitarPlaylistsQuery
) : IQueryHandler<GetPlaylistsQuery, IReadOnlyList<GuitarPlaylistResponse>>
{
    public async Task<Result<IReadOnlyList<GuitarPlaylistResponse>>> Handle(
        GetPlaylistsQuery request,
        CancellationToken cancellationToken
    )
    {
        var playlists = await guitarPlaylistsQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        IReadOnlyList<GuitarPlaylistResponse> response = playlists
            .Select(playlist => new GuitarPlaylistResponse(
                playlist.Id.Value,
                playlist.Name,
                playlist.CreatedOnUtc,
                playlist.ModifiedOnUtc
            ))
            .ToList();

        return Result.Success(response);
    }
}
