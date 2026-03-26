using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.UpdatePlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Commands;

internal sealed record UpdatePlaylistRequest(string Name);

internal sealed class UpdatePlaylist(ISender sender) : Endpoint<UpdatePlaylistRequest, IResult>
{
    public override void Configure()
    {
        Put("{playlistId:guid}");
        Group<LibraryPlaylistsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(UpdatePlaylistRequest req, CancellationToken ct)
    {
        var playlistId = YoutubePlaylistId.Create(Route<Guid>("playlistId"));

        return await Result
            .Create(new UpdatePlaylistCommand(playlistId, req.Name))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
