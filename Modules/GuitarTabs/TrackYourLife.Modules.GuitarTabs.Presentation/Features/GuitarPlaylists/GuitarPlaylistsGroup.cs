namespace TrackYourLife.Modules.GuitarTabs.Presentation.Features.GuitarPlaylists;

internal sealed class GuitarPlaylistsGroup : Group
{
    public GuitarPlaylistsGroup()
    {
        Configure(
            ApiRoutes.GuitarPlaylists,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
