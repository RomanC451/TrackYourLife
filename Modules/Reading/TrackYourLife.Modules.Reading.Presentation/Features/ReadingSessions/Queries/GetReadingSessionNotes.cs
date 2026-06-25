using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetReadingSessionNotes;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Queries;

internal sealed class GetReadingSessionNotes(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/{id}/notes");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces<List<ReadingSessionNoteDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var sessionId = Route<ReadingSessionId>("id")!;

        return await Result
            .Create(new GetReadingSessionNotesQuery(sessionId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync();
    }
}
