using TrackYourLife.Modules.Reading.Application.Features.Books.Commands.CreateBook;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Contracts.Shared;

namespace TrackYourLife.Modules.Reading.Presentation.Features.Books.Commands;

internal sealed record CreateBookRequest(
    string Title,
    string Author,
    int TotalPages,
    int CurrentPage,
    BookStatus Status,
    DateOnly? StartingDate,
    DateOnly? FinishDate,
    int? Rating
);

internal sealed class CreateBook(ISender sender) : Endpoint<CreateBookRequest, IResult>
{
    public override void Configure()
    {
        Post("");
        Group<BooksGroup>();
        Description(x =>
            x.Produces<IdResponse>(StatusCodes.Status201Created)
                .ProducesProblemFE<ProblemDetails>(StatusCodes.Status400BadRequest)
        );
    }

    public override async Task<IResult> ExecuteAsync(CreateBookRequest req, CancellationToken ct)
    {
        return await Result
            .Create(
                new CreateBookCommand(
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
            .ToCreatedActionResultAsync(bookId => $"/{ApiRoutes.Books}/{bookId.Value}");
    }
}
