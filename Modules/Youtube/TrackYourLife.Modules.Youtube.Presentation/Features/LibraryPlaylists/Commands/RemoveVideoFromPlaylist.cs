using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Commands.RemoveVideoFromPlaylist;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Commands;

internal sealed class RemoveVideoFromPlaylist(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("{playlistId:guid}/videos/{youtubeId}");
        Group<LibraryPlaylistsGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var playlistId = YoutubePlaylistId.Create(Route<Guid>("playlistId"));
        var youtubeId = Route<string>("youtubeId");

        if (string.IsNullOrWhiteSpace(youtubeId))
        {
            return Results.BadRequest("YouTube video ID is required.");
        }

        return await Result
            .Create(new RemoveVideoFromPlaylistCommand(playlistId, youtubeId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
