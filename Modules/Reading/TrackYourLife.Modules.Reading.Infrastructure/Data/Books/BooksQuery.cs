using Microsoft.EntityFrameworkCore;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.Books;

internal sealed class BooksQuery(ReadingReadDbContext context) : IBooksQuery
{
    public async Task<IReadOnlyList<BookReadModel>> GetByUserIdAsync(
        UserId userId,
        BookStatus? status,
        BookSortField sortField,
        bool sortDescending,
        CancellationToken cancellationToken = default
    )
    {
        var query = context.Books.Where(b => b.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(b => b.Status == status.Value);
        }

        query = sortField switch
        {
            BookSortField.Title => sortDescending
                ? query.OrderByDescending(b => b.Title)
                : query.OrderBy(b => b.Title),
            BookSortField.Author => sortDescending
                ? query.OrderByDescending(b => b.Author)
                : query.OrderBy(b => b.Author),
            BookSortField.StartingDate => sortDescending
                ? query.OrderByDescending(b => b.StartingDate)
                : query.OrderBy(b => b.StartingDate),
            BookSortField.FinishDate => sortDescending
                ? query.OrderByDescending(b => b.FinishDate)
                : query.OrderBy(b => b.FinishDate),
            BookSortField.Rating => sortDescending
                ? query.OrderByDescending(b => b.Rating)
                : query.OrderBy(b => b.Rating),
            _ => query.OrderBy(b => b.Title),
        };

        return await query.ToListAsync(cancellationToken);
    }

    public Task<BookReadModel?> GetByIdAsync(
        BookId id,
        CancellationToken cancellationToken = default
    ) => context.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
}
