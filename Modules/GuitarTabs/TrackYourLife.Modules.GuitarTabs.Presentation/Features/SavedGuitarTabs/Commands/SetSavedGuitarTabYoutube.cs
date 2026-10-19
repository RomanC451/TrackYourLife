using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Commands.SetGuitarTabYoutube;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.SavedGuitarTabs.Commands;

internal sealed record SetSavedGuitarTabYoutubeRequest(
    string TabUrl,
    string SongName,
    string ArtistName,
    string Type,
    string? YoutubeUrl
);

internal sealed class SetSavedGuitarTabYoutube(ISender sender)
    : Endpoint<SetSavedGuitarTabYoutubeRequest, IResult>
{
    public override void Configure()
    {
        Put("youtube");
        Group<SavedGuitarTabsGroup>();
        Description(x =>
            x.Produces<GuitarTabLibraryStateResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SetSavedGuitarTabYoutubeRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(
                new SetGuitarTabYoutubeCommand(
                    req.TabUrl,
                    req.SongName,
                    req.ArtistName,
                    req.Type,
                    req.YoutubeUrl
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(response => response);
    }
}
