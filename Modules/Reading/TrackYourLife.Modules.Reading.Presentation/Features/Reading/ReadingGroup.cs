namespace TrackYourLife.Modules.Reading.Presentation.Features.Reading;

internal sealed class ReadingGroup : Group
{
    public ReadingGroup()
    {
        Configure(
            ApiRoutes.Reading,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
