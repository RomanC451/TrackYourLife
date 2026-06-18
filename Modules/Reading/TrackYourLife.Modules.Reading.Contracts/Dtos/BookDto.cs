using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Contracts.Dtos;

public sealed record BookDto(
    Guid Id,
    string Title,
    string Author,
    int TotalPages,
    int CurrentPage,
    BookStatus Status,
    DateOnly? StartingDate,
    DateOnly? FinishDate,
    int? Rating,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool SuggestMarkAsFinished
);
