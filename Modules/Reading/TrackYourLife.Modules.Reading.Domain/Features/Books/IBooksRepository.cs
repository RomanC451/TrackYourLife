using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Domain.Features.Books;

public interface IBooksRepository
{
    Task<Book?> GetByIdAsync(BookId id, CancellationToken cancellationToken = default);

    Task AddAsync(Book book, CancellationToken cancellationToken = default);

    void Update(Book book);

    void Remove(Book book);
}
