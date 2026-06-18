using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Commands.CreateBook;

internal sealed class CreateBookCommandHandler(
    IBooksRepository booksRepository,
    IUserIdentifierProvider userIdentifierProvider
) : ICommandHandler<CreateBookCommand, BookId>
{
    public async Task<Result<BookId>> Handle(
        CreateBookCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = Book.Create(
            BookId.NewId(),
            userIdentifierProvider.UserId,
            command.Title,
            command.Author,
            command.TotalPages,
            command.CurrentPage,
            command.Status,
            command.StartingDate,
            command.FinishDate,
            command.Rating
        );

        if (result.IsFailure)
        {
            return Result.Failure<BookId>(result.Error);
        }

        await booksRepository.AddAsync(result.Value, cancellationToken);

        return Result.Success(result.Value.Id);
    }
}
