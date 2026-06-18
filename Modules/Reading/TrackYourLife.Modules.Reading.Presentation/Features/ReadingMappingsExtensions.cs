using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Contracts.MappingsExtensions;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Presentation.Features;

internal static class ReadingMappingsExtensions
{
    public static BookDto ToDto(this BookReadModel book) =>
        new(
            book.Id.Value,
            book.Title,
            book.Author,
            book.TotalPages,
            book.CurrentPage,
            book.Status,
            book.StartingDate,
            book.FinishDate,
            book.Rating,
            book.CreatedOnUtc,
            book.ModifiedOnUtc,
            book.SuggestMarkAsFinished()
        );

    public static ReadingSessionDto ToDto(this ReadingSessionReadModel session) =>
        new(
            session.Id.Value,
            session.BookId.Value,
            session.BookTitle,
            session.BookAuthor,
            session.SessionDate,
            session.StartPage,
            session.EndPage,
            session.PagesRead,
            session.DurationSeconds,
            session.Notes,
            session.StartedOnUtc,
            session.FinishedOnUtc,
            session.CreatedOnUtc,
            session.IsActive
        );

    public static BookNoteDto ToNoteDto(this ReadingSessionNoteReadModel note) =>
        new(
            note.Id.Value,
            note.ReadingSessionId.Value,
            note.SessionDate ?? DateOnly.FromDateTime(note.CreatedOnUtc),
            note.ChapterTitle,
            note.Content
        );
}
