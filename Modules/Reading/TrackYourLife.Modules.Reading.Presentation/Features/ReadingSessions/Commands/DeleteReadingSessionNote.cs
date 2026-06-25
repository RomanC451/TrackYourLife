using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.DeleteReadingSessionNote;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed class DeleteReadingSessionNote(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("/{sessionId}/notes/{noteId}");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces(StatusCodes.Status204NoContent));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var sessionId = Route<ReadingSessionId>("sessionId")!;
        var noteId = Route<ReadingSessionNoteId>("noteId")!;

        return await Result
            .Create(new DeleteReadingSessionNoteCommand(sessionId, noteId))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
