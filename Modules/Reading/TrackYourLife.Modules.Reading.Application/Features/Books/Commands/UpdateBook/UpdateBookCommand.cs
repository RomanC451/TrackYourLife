using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Commands.UpdateBook;

public sealed record UpdateBookCommand(
    BookId Id,
    string Title,
    string Author,
    int TotalPages,
    int CurrentPage,
    BookStatus Status,
    DateOnly? StartingDate,
    DateOnly? FinishDate,
    int? Rating
) : ICommand;
