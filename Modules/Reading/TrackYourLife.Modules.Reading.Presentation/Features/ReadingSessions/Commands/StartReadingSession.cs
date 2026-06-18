using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.StartReadingSession;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed record StartReadingSessionRequest(Guid BookId);

internal sealed class StartReadingSession(ISender sender)
    : Endpoint<StartReadingSessionRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<ReadingSessionsGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status409Conflict)
        );
    }

    public override async Task<IResult> ExecuteAsync(
        StartReadingSessionRequest req,
        CancellationToken ct
    )
    {
        return await Result
            .Create(new StartReadingSessionCommand(BookId.Create(req.BookId)))
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(id => $"/{ApiRoutes.ReadingSessions}/{id.Value}");
    }
}
