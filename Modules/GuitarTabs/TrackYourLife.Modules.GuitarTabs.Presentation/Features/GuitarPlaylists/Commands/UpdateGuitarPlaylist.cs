using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarPlaylists.Commands.UpdatePlaylist;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists.Commands;

internal sealed record UpdatePlaylistRequest(string Name);

internal sealed class UpdateGuitarPlaylist(ISender sender) : Endpoint<UpdatePlaylistRequest, IResult>
{
    public override void Configure()
    {
        Put("{id:guid}");
        Group<GuitarPlaylistsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(UpdatePlaylistRequest req, CancellationToken ct)
    {
        var playlistId = GuitarPlaylistId.Create(Route<Guid>("id"));

        return await Result
            .Create(new UpdatePlaylistCommand(playlistId, req.Name))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
