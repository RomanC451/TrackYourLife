using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Queries.GetBookById;

internal sealed class GetBookByIdQueryHandler(
    IBooksQuery booksQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetBookByIdQuery, BookReadModel>
{
    public async Task<Result<BookReadModel>> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var book = await booksQuery.GetByIdAsync(request.Id, cancellationToken);

        if (book is null)
        {
            return Result.Failure<BookReadModel>(BookErrors.NotFound(request.Id));
        }

        if (book.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<BookReadModel>(BookErrors.NotOwned(request.Id));
        }

        return Result.Success(book);
    }
}
