using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.RemoveGuitarTabFromPlaylist;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarPlaylists;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.SavedGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists.Commands;

internal sealed class DeleteGuitarPlaylistSong(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Delete("{playlistId:guid}/songs/{songId:guid}");
        Group<GuitarPlaylistsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var playlistId = GuitarPlaylistId.Create(Route<Guid>("playlistId"));
        var songId = SavedGuitarTabId.Create(Route<Guid>("songId"));

        return await Result
            .Create(new RemoveGuitarTabFromPlaylistCommand(playlistId, songId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
