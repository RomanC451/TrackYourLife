namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record ReadingSessionNoteDto(
    Guid Id,
    string ChapterTitle,
    string Content,
    DateTime CreatedOnUtc
);
