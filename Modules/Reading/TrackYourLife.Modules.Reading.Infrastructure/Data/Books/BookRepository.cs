using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Infrastructure.Data;

namespace TrackYourLife.Modules.Reading.Infrastructure.Data.Books;

internal sealed class BookRepository(ReadingWriteDbContext context)
    : GenericRepository<Book, BookId>(context.Books),
        IBooksRepository;
