namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record BookNoteDto(
    Guid NoteId,
    Guid SessionId,
    DateOnly SessionDate,
    string ChapterTitle,
    string Content
);
