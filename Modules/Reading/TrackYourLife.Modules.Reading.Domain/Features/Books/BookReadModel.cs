using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;

namespace TrackYourLife.Modules.Reading.Domain.Features.Books;

public sealed record BookReadModel(
    BookId Id,
    UserId UserId,
    string Title,
    string Author,
    int TotalPages,
    int CurrentPage,
    BookStatus Status,
    DateOnly? StartingDate,
    DateOnly? FinishDate,
    int? Rating,
    DateTime CreatedOnUtc,
    DateTime? ModifiedOnUtc
) : IReadModel<BookId>;
