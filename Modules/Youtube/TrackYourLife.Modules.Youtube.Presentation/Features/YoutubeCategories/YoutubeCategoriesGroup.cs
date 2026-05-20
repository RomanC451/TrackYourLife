using TrackYourLife.Modules.Youtube.Presentation.Contracts;

namespace TrackYourLife.Modules.Youtube.Presentation.Features.YoutubeCategories;

internal sealed class YoutubeCategoriesGroup : Group
{
    public YoutubeCategoriesGroup()
    {
        Configure(
            ApiRoutes.SettingsCategories,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
