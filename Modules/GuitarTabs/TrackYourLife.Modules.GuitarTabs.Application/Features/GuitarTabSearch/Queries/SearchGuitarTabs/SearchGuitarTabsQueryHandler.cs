namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.SearchGuitarTabs;

internal sealed class SearchGuitarTabsQueryHandler(IUltimateGuitarSearchClient searchClient)
    : IQueryHandler<SearchGuitarTabsQuery, GuitarTabSearchResponse>
{
    public Task<Result<GuitarTabSearchResponse>> Handle(
        SearchGuitarTabsQuery request,
        CancellationToken cancellationToken
    )
    {
        return searchClient.SearchByTitleAsync(request.Value.Trim(), request.Page, cancellationToken);
    }
}
