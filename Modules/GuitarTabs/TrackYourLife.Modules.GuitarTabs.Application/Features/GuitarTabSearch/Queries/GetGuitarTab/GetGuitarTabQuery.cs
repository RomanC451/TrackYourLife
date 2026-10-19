namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabSearch.Queries.GetGuitarTab;

public sealed record GetGuitarTabQuery(string Url) : IQuery<GuitarTabContentResponse>;
