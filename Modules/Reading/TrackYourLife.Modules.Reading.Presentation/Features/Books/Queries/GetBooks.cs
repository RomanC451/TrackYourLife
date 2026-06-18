using TrackYourLife.Modules.Reading.Application.Features.Books.Queries.GetBooks;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Books.Queries;

internal sealed record GetBooksRequest
{
    [QueryParam]
    public BookStatus? Status { get; init; }

    [QueryParam]
    public BookSortField? SortBy { get; init; }

    [QueryParam]
    public bool? SortDescending { get; init; }
}

internal sealed class GetBooks(ISender sender) : Endpoint<GetBooksRequest, IResult>
{
    public override void Configure()
    {
        Get("");
        Group<BooksGroup>();
        Description(x => x.Produces<List<BookDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(GetBooksRequest req, CancellationToken ct)
    {
        return await Result
            .Create(
                new GetBooksQuery(
                    req.Status,
                    req.SortBy ?? BookSortField.Title,
                    req.SortDescending ?? false
                )
            )
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(books => books.Select(b => b.ToDto()).ToList());
    }
}
