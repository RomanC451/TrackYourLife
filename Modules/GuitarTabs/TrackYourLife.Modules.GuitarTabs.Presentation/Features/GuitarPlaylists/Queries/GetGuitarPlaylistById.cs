using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Models;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Queries.GetPlaylistById;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists.Queries;

internal sealed class GetGuitarPlaylistById(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Get("{id:guid}");
        Group<GuitarPlaylistsGroup>();
        Description(x =>
            x.Produces<GuitarPlaylistResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var playlistId = GuitarPlaylistId.Create(Route<Guid>("id"));

        return await Result
            .Create(new GetPlaylistByIdQuery(playlistId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(playlist => playlist);
    }
}
