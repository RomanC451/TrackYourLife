using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.Books;

public enum BookSortField
{
    Title,
    Author,
    StartingDate,
    FinishDate,
    Rating,
}

public interface IBooksQuery
{
    Task<IReadOnlyList<BookReadModel>> GetByUserIdAsync(
        UserId userId,
        BookStatus? status,
        BookSortField sortField,
        bool sortDescending,
        CancellationToken cancellationToken = default
    );

    Task<BookReadModel?> GetByIdAsync(BookId id, CancellationToken cancellationToken = default);
}
