namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.SavedGuitarTabs;

internal sealed class SavedGuitarTabsGroup : Group
{
    public SavedGuitarTabsGroup()
    {
        Configure(
            ApiRoutes.SavedGuitarTabs,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
