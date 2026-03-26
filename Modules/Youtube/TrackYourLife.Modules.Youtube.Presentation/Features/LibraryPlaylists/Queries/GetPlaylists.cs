using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Models;
using TrackYourLife.Modules.Youtube.Application.Features.YoutubePlaylists.Queries.GetPlaylists;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists.Queries;

internal sealed class GetPlaylists(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<LibraryPlaylistsGroup>();
        Description(x =>
            x.Produces<IEnumerable<YoutubePlaylistWithVideoPreviews>>(StatusCodes.Status200OK)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetPlaylistsQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(x => x);
    }
}
