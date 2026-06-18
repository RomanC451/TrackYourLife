using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Queries.GetBooks;

public sealed record GetBooksQuery(
    BookStatus? Status,
    BookSortField SortField,
    bool SortDescending
) : IQuery<IReadOnlyList<BookReadModel>>;
