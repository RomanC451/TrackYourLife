namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record BookChapterNoteEntryDto(
    Guid NoteId,
    Guid SessionId,
    DateOnly Date,
    string Content,
    DateTime CreatedOnUtc
);

public sealed record BookChapterNotesGroupDto(
    string ChapterTitle,
    string? PageRange,
    IReadOnlyList<BookChapterNoteEntryDto> Notes
);
