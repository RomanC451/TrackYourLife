using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Models;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Queries.GetPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists.Queries;

internal sealed class GetGuitarPlaylists(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<GuitarPlaylistsGroup>();
        Description(x => x.Produces<IEnumerable<GuitarPlaylistResponse>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetPlaylistsQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(playlists => playlists);
    }
}
