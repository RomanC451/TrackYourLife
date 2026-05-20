using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.DeletePlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Commands;

internal sealed class DeletePlaylist(ISender sender) : Endpoint<EmptyRequest, IResult>
{
    public override void Configure()
    {
        Delete("{id:guid}");
        Group<LibraryPlaylistsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(EmptyRequest req, CancellationToken ct)
    {
        var playlistId = YoutubePlaylistId.Create(Route<Guid>("id"));

        return await Result
            .Create(new DeletePlaylistCommand(playlistId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
