using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch;
using TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.SearchGuitarTabs;

namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarTabSearch.Queries;

internal sealed class SearchGuitarTabsRequest
{
    [QueryParam]
    public string Value { get; init; } = string.Empty;

    [QueryParam]
    public int Page { get; init; } = 1;
}

internal sealed class SearchGuitarTabs(ISender sender)
    : Endpoint<SearchGuitarTabsRequest, IResult>
{
    public override void Configure()
    {
        Get(ApiRoutes.GuitarTabSearch);
        Description(x =>
            x.Produces<GuitarTabSearchResponse>(StatusCodes.Status200OK)
                .ProducesProblem(StatusCodes.Status401Unauthorized)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status502BadGateway)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        SearchGuitarTabsRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new SearchGuitarTabsQuery(req.Value.Trim(), req.Page))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(response => response);
    }
}
