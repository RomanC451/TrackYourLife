using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabFavorite;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.SavedGuitarTabs.Commands;

internal sealed record SetSavedGuitarTabFavoriteRequest(
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type,
    bool IsFavorite
);

internal sealed class SetSavedGuitarTabFavorite(ISender sender)
    : Endpoint<SetSavedGuitarTabFavoriteRequest, IResult>
{
    public override void Configure()
    {
        Put("favorite");
        Group<SavedGuitarTabsGroup>();
        Description(x =>
            x.Produces<GuitarTabLibraryStateResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SetSavedGuitarTabFavoriteRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new SetGuitarTabFavoriteCommand(
                    req.TabUrl,
                    req.SongName,
                    req.ArtistName,
                    req.Type,
                    req.IsFavorite
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(response => response);
    }
}
