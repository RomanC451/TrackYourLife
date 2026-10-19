using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.DeletePlaylist;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists.Commands;

internal sealed class DeleteGuitarPlaylist(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Delete("{id:guid}");
        Group<GuitarPlaylistsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var playlistId = GuitarPlaylistId.Create(Route<Guid>("id"));

        return await Result
            .Create(new DeletePlaylistCommand(playlistId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
