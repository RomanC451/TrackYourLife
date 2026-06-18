using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Commands.UpdateBook;

internal sealed class UpdateBookCommandHandler(
    IBooksRepository booksRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<UpdateBookCommand>
{
    public async Task<Result> Handle(
        UpdateBookCommand command,
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

        var updateResult = book.Update(
            command.Title,
            command.Author,
            command.TotalPages,
            command.CurrentPage,
            command.Status,
            command.StartingDate,
            command.FinishDate,
            command.Rating
        );

        if (updateResult.IsFailure)
        {
            return updateResult;
        }

        booksRepository.Update(book);

        return Result.Success();
    }
}
