using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.LogReadingProgress;
using TrackYourLife.Modules.Reading.Contracts.Dtos;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed record LogReadingProgressRequest(int EndPage, DateOnly? SessionDate);

internal sealed class LogReadingProgress(ISender sender)
    : Endpoint<LogReadingProgressRequest, IResult>
{
    public override void Configure()
    {
        Post("/log-progress");
        Group<ReadingSessionsGroup>();
        Description(x =>
            x.Produces<LogReadingProgressDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        LogReadingProgressRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(req, DomainErrors.General.UnProcessableRequest)
            .Map(r => new LogReadingProgressCommand(r.EndPage, r.SessionDate))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync(dto => dto);
    }
}
