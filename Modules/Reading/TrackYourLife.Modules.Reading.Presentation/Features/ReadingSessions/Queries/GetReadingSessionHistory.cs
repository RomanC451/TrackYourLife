using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetReadingSessionHistory;
using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Queries;

internal sealed class GetReadingSessionHistory(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces<List<ReadingSessionDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new GetReadingSessionHistoryQuery())
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(sessions => sessions.Select(s => s.ToDto()).ToList());
    }
}
