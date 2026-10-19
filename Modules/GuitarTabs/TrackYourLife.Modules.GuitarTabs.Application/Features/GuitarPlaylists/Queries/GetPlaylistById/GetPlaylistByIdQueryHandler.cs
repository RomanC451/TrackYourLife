using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Models;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Queries.GetPlaylistById;

internal sealed class GetPlaylistByIdQueryHandler(
    IUserIdentifierProvider userIdentifierProvider,
    IGuitarPlaylistsQuery guitarPlaylistsQuery
) : IQueryHandler<GetPlaylistByIdQuery, GuitarPlaylistResponse>
{
    public async Task<Result<GuitarPlaylistResponse>> Handle(
        GetPlaylistByIdQuery request,
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
            return Result.Failure<GuitarPlaylistResponse>(
                GuitarPlaylistErrors.NotFound(request.GuitarPlaylistId.Value)
            );
        }

        return Result.Success(
            new GuitarPlaylistResponse(
                playlist.Id.Value,
                playlist.Name,
                playlist.CreatedOnUtc,
                playlist.ModifiedOnUtc
            )
        );
    }
}
