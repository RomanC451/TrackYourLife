using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Commands.DeleteBook;

internal sealed class DeleteBookCommandHandler(
    IBooksRepository booksRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<DeleteBookCommand>
{
    public async Task<Result> Handle(
        DeleteBookCommand command,
        CancellationToken cancellationToken
    )
    {
        var book = await booksRepository.GetByIdAsync(command.Id, cancellationToken);

        if (book is null)
        {
            return Result.Failure(BookErrors.NotFound(command.Id));
        }

        if (book.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure(BookErrors.NotOwned(command.Id));
        }

        booksRepository.Remove(book);

        return Result.Success();
    }
}
