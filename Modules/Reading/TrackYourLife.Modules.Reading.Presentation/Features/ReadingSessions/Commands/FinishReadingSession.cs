using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.FinishReadingSession;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed record FinishReadingSessionRequest(
    int EndPage,
    DateOnly? SessionDate,
    string? Notes,
    int? DurationSeconds
);

internal sealed record FinishReadingSessionResponse(bool SuggestMarkAsFinished);

internal sealed class FinishReadingSession(ISender sender)
    : Endpoint<FinishReadingSessionRequest, IResult>
{
    public override void Configure()
    {
        Post("/{id}/finish");
        Group<ReadingSessionsGroup>();
        Description(x =>
            x.Produces<FinishReadingSessionResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        FinishReadingSessionRequest req,
        CancellationToken ct
    )
    {
        var id = Route<ReadingSessionId>("id")!;

        return await Result
            .Create(
                new FinishReadingSessionCommand(
                    id,
                    req.EndPage,
                    req.SessionDate,
                    req.Notes,
                    req.DurationSeconds
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(suggest => new FinishReadingSessionResponse(suggest));
    }
}
