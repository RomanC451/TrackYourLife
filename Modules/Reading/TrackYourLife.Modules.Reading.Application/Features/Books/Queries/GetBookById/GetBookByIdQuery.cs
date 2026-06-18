using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Application.Features.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery(BookId Id) : IQuery<BookReadModel>;
