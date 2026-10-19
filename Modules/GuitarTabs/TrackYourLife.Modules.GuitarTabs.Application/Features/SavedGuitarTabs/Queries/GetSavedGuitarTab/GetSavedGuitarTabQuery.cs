namespace TrackYourLife.Modules.GuitarTabs.Application.Features.SavedGuitarTabs.Queries.GetSavedGuitarTab;

public sealed record GetSavedGuitarTabQuery(string Url) : IQuery<GuitarTabLibraryStateResponse>;
