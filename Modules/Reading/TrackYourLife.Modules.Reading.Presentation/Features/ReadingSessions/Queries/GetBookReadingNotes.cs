using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetBookReadingNotes;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Books;

namespace TrackYourLife.Modules.Reading.Presentation.Features.ReadingSessions.Queries;

internal sealed class GetBookReadingNotes(ISender sender) : EndpointWithoutRequest<IResult>
{
    public override void Configure()
    {
        Get("/by-book/{bookId}");
        Group<ReadingSessionsGroup>();
        Description(x => x.Produces<List<BookChapterNotesGroupDto>>(StatusCodes.Status200OK));
    }

    public override async Task<IResult> ExecuteAsync(CancellationToken ct)
    {
        var bookId = Route<BookId>("bookId")!;

        return await Result
            .Create(new GetBookReadingNotesQuery(bookId))
            .BindAsync(query => sender.Send(query, ct))
            .ToActionResultAsync();
    }
}
