using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.UpdateReadingSessionNote;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed record UpdateReadingSessionNoteRequest(string ChapterTitle, string Content);

internal sealed class UpdateReadingSessionNote(ISender sender)
    : Endpoint<UpdateReadingSessionNoteRequest, IResult>
{
    public override void Configure()
    {
        Put("/{sessionId}/notes/{noteId}");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces(StatusCodes.Status204NoContent));
    }

    public override async Task<IResult> ExecuteAsync(
        UpdateReadingSessionNoteRequest req,
        CancellationToken ct
    )
    {
        var sessionId = Route<ReadingSessionId>("sessionId")!;
        var noteId = Route<ReadingSessionNoteId>("noteId")!;

        return await Result
            .Create(
                new UpdateReadingSessionNoteCommand(
                    sessionId,
                    noteId,
                    req.ChapterTitle,
                    req.Content
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
