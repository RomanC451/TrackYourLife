using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Commands.DeleteBook;

public sealed record DeleteBookCommand(BookId Id) : ICommand;
