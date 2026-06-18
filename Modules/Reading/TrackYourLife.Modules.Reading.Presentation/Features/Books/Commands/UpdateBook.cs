using TrackYourLife.Modules.Reading.Application.Features.Books.Commands.UpdateBook;
using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Books.Commands;

internal sealed record UpdateBookRequest(
    string Title,
    string Author,
    int TotalPages,
    int CurrentPage,
    BookStatus Status,
    DateOnly? StartingDate,
    DateOnly? FinishDate,
    int? Rating
);

internal sealed class UpdateBook(ISender sender) : Endpoint<UpdateBookRequest, IResult>
{
    public override void Configure()
    {
        Put("/{id}");
        Group<BooksGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(UpdateBookRequest req, CancellationToken ct)
    {
        var id = Route<BookId>("id")!;

        return await Result
            .Create(
                new UpdateBookCommand(
                    id,
                    req.Title,
                    req.Author,
                    req.TotalPages,
                    req.CurrentPage,
                    req.Status,
                    req.StartingDate,
                    req.FinishDate,
                    req.Rating
                )
            )
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
