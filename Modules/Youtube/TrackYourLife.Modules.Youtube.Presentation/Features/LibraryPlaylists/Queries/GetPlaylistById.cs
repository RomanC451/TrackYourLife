using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylistById;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubePlaylists;
using TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Models;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Queries;

internal sealed class GetPlaylistById(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("{playlistId:guid}");
        Group<LibraryPlaylistsGroup>();
        Description(x =>
            x.Produces<YoutubePlaylistDetailDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var playlistId = YoutubePlaylistId.Create(Route<Guid>("playlistId"));

        return await Result
            .Create(new GetPlaylistByIdQuery(playlistId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(result => result.ToDetailDto());
    }
}
