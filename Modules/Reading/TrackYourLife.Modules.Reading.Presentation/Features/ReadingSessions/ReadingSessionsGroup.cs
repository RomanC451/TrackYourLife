namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions;

internal sealed class ReadingSessionsGroup : Group
{
    public ReadingSessionsGroup()
    {
        Configure(
            ApiRoutes.ReadingSessions,
            ep =>
            {
                ep.Description(x => x.ProducesProblem(StatusCodes.Status401Unauthorized));
            }
        );
    }
}
