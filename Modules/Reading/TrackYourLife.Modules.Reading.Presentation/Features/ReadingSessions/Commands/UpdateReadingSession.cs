using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.UpdateReadingSession;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed record UpdateReadingSessionRequest(
    int EndPage,
    DateOnly SessionDate,
    int? DurationSeconds
);

internal sealed class UpdateReadingSession(ISender sender)
    : Endpoint<UpdateReadingSessionRequest, IResult>
{
    public override void Configure()
    {
        Put("/{id}");
        Group<ReadingSessionsGroup>();
        Description(x =>
            x.Produces<FinishReadingSessionResponse>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateReadingSessionRequest req,
        CancellationToken ct
    )
    {
        var id = Route<ReadingSessionId>("id")!;

        return await Result
            .Create(
                new UpdateReadingSessionCommand(
                    id,
                    req.EndPage,
                    req.SessionDate,
                    req.DurationSeconds
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(suggest => new FinishReadingSessionResponse(suggest));
    }
}
