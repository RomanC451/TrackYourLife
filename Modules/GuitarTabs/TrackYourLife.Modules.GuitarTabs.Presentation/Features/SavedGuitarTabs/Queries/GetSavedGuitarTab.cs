using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs;
using TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetSavedGuitarTab;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.SavedGuitarTabs.Queries;

internal sealed class GetSavedGuitarTabRequest
{
    [QueryParam]
    public string Url { get; init; } = string.Empty;
}

internal sealed class GetSavedGuitarTab(ISender sender) : Endpoint<GetSavedGuitarTabRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<SavedGuitarTabsGroup>();
        Description(x => x.Produces<GuitarTabLibraryStateResponse>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(GetSavedGuitarTabRequest req, CancellationToken ct)
    {
        return await Result
            .Create(new GetSavedGuitarTabQuery(req.Url.Trim()))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(response => response);
    }
}
