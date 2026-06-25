namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record RandomReadingNoteDto(
    Guid NoteId,
    Guid BookId,
    string BookTitle,
    string ChapterTitle,
    string Content
);
