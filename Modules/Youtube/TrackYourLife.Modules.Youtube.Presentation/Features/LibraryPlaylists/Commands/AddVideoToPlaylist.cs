using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.AddVideoToPlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Commands;

internal sealed record AddVideoToPlaylistRequest(string VideoId);

internal sealed class AddVideoToPlaylist(ISender sender)
    : Endpoint<AddVideoToPlaylistRequest, IResult>
{
    public override void Configure()
    {
        Post("{playlistId:guid}/videos");
        Group<LibraryPlaylistsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        AddVideoToPlaylistRequest req,
        CancellationToken ct
    )
    {
        var playlistId = YoutubePlaylistId.Create(Route<Guid>("playlistId"));

        return await Result
            .Create(new AddVideoToPlaylistCommand(playlistId, req.VideoId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
