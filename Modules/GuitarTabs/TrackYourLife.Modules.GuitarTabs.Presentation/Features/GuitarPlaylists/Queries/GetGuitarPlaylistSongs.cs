using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetPlaylistSongs;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists.Queries;

internal sealed class GetGuitarPlaylistSongs(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Get("{id:guid}/songs");
        Group<GuitarPlaylistsGroup>();
        Description(x =>
            x.Produces<IReadOnlyList<SavedGuitarTabResponse>>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var playlistId = GuitarPlaylistId.Create(Route<Guid>("id"));

        return await Result
            .Create(new GetPlaylistSongsQuery(playlistId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(response => response);
    }
}
