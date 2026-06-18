using TrackYourLife.Modules.Reading.Application.Features.Books.Queries.GetBookById;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Books.Queries;

internal sealed class GetBookById(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/{id}");
        Group<BooksGroup>();
        Description(x =>
            x.Produces<BookDto>(StatusCodes.Status200OK)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var id = Route<BookId>("id")!;

        return await Result
            .Create(new GetBookByIdQuery(id))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync(book => book.ToDto());
    }
}
