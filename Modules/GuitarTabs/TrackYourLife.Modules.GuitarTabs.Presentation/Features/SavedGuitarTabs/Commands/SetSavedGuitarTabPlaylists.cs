using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabPlaylists;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.SavedGuitarTabs.Commands;

internal sealed record SetSavedGuitarTabPlaylistsRequest(
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type,
    IReadOnlyList<Guid> PlaylistIds
);

internal sealed class SetSavedGuitarTabPlaylists(ISender sender)
    : Endpoint<SetSavedGuitarTabPlaylistsRequest, IResult>
{
    public override void Configure()
    {
        Put("playlists");
        Group<SavedGuitarTabsGroup>();
        Description(x =>
            x.Produces<GuitarTabLibraryStateResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SetSavedGuitarTabPlaylistsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new SetGuitarTabPlaylistsCommand(
                    req.TabUrl,
                    req.SongName,
                    req.ArtistName,
                    req.Type,
                    req.PlaylistIds
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(response => response);
    }
}
