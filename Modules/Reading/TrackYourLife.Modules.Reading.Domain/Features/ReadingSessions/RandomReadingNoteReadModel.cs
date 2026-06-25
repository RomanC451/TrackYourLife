using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public sealed record RandomReadingNoteReadModel(
    ReadingSessionNoteId Id,
    BookId BookId,
    string BookTitle,
    string ChapterTitle,
    string Content
);
