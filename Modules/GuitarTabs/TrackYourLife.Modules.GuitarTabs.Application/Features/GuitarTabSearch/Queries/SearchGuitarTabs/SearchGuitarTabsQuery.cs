namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.SearchGuitarTabs;

public sealed record SearchGuitarTabsQuery(string Value, int Page = 1)
    : IQuery<GuitarTabSearchResponse>;
