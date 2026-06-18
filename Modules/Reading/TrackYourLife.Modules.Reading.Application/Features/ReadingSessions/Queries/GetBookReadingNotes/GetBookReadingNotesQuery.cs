using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetBookReadingNotes;

public sealed record GetBookReadingNotesQuery(BookId BookId)
    : IQuery<IReadOnlyList<BookChapterNotesGroupDto>>;
