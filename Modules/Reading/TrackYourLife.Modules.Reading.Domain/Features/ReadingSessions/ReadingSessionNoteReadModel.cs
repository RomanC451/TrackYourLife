using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public sealed record ReadingSessionNoteReadModel(
    ReadingSessionNoteId Id,
    ReadingSessionId ReadingSessionId,
    BookId BookId,
    UserId UserId,
    string ChapterTitle,
    string Content,
    DateTime CreatedOnUtc,
    DateOnly? SessionDate
) : IReadModel<ReadingSessionNoteId>;
