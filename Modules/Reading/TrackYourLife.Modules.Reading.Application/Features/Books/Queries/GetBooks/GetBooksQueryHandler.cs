using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Queries.GetBooks;

internal sealed class GetBooksQueryHandler(
    IBooksQuery booksQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetBooksQuery, IReadOnlyList<BookReadModel>>
{
    public async Task<Result<IReadOnlyList<BookReadModel>>> Handle(
        GetBooksQuery request,
        CancellationToken cancellationToken
    )
    {
        var books = await booksQuery.GetByUserIdAsync(
            userIdentifierProvider.UserId,
            request.Status,
            request.SortField,
            request.SortDescending,
            cancellationToken
        );

        return Result.Success(books);
    }
}
