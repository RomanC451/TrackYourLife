using TrackYourLife.Modules.Reading.Application.Features.Books.Commands.DeleteBook;
using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Books.Commands;

internal sealed class DeleteBook(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Delete("/{id}");
        Group<BooksGroup>();
        Description(x =>
            x.Produces(StatusCodes.Status204NoContent)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status404NotFound)
        );
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        return await Result
            .Create(new DeleteBookCommand(Route<BookId>("id")!))
            .BindAsync(command => sender.Send(command, ct))
            .ToActionResultAsync();
    }
}
