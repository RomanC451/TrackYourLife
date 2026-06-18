using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.AddReadingSessionNote;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Commands;

internal sealed record AddReadingSessionNoteRequest(string ChapterTitle, string Content);

internal sealed class AddReadingSessionNote(ISender sender)
    : Endpoint<AddReadingSessionNoteRequest, IResult>
{
    public override void Configure()
    {
        Post("/{id}/notes");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces<IdResponse>(StatusCodes.Status201Created));
    }

    public override async Task<IResult> ExecuteAsync(
        AddReadingSessionNoteRequest req,
        CancellationToken ct
    )
    {
        var id = Route<ReadingSessionId>("id")!;

        return await Result
            .Create(
                new AddReadingSessionNoteCommand(id, req.ChapterTitle, req.Content)
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToCreatedActionResultAsync(noteId =>
                $"/{ApiRoutes.ReadingSessions}/{id.Value}/notes/{noteId.Value}"
            );
    }
}
