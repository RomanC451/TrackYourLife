using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetActiveReadingSession;
using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Queries;

internal sealed class GetActiveReadingSession(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/active");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces<ReadingSessionDto>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetActiveReadingSessionQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(session => session?.ToDto());
    }
}
