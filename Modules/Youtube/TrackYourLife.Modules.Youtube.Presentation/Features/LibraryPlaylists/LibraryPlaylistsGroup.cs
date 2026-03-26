using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.LibraryPlaylists;

internal sealed class LibraryPlaylistsGroup : Group
{
    public LibraryPlaylistsGroup()
    {
        Configure(
            ApiRoutes.LibraryPlaylists,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
