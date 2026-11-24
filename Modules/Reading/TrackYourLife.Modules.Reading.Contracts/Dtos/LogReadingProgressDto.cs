using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record LogReadingProgressDto(
    ReadingSessionId SessionId,
    BookId BookId,
    string BookTitle,
    int StartPage,
    int EndPage,
    int PagesRead,
    bool SuggestMarkAsFinished
);
