using TrackYourLife.Modules.Reading.Application.Abstraction;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetRandomReadingNote;

internal sealed class GetRandomReadingNoteQueryHandler(
    IReadingSessionNotesQuery readingSessionNotesQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetRandomReadingNoteQuery, RandomReadingNoteDto>
{
    public async Task<Result<RandomReadingNoteDto>> Handle(
        GetRandomReadingNoteQuery request,
        CancellationToken cancellationToken
    )
    {
        var note = await readingSessionNotesQuery.GetRandomByUserIdAsync(
            userIdentifierProvider.UserId,
            cancellationToken
        );

        if (note is null)
        {
            return Result.Failure<RandomReadingNoteDto>(
                ReadingSessionNoteErrors.NoneAvailable
            );
        }

        return Result.Success(
            new RandomReadingNoteDto(
                note.Id.Value,
                note.BookId.Value,
                note.BookTitle,
                note.ChapterTitle,
                note.Content
            )
        );
    }
}
