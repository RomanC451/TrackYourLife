using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.CancelReadingSession;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed class CancelReadingSession(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("/{id}");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces(StatusCodes.Status204NoContent));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new CancelReadingSessionCommand(Route<ReadingSessionId>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
