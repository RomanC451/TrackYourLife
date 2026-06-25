using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetReadingSessionNotes;

internal sealed class GetReadingSessionNotesQueryHandler(
    IReadingSessionsQuery readingSessionsQuery,
    IReadingSessionNotesQuery readingSessionNotesQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetReadingSessionNotesQuery, IReadOnlyList<ReadingSessionNoteDto>>
{
    public async Task<Result<IReadOnlyList<ReadingSessionNoteDto>>> Handle(
        GetReadingSessionNotesQuery request,
        CancellationToken cancellationToken
    )
    {
        var session = await readingSessionsQuery.GetByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return Result.Failure<IReadOnlyList<ReadingSessionNoteDto>>(
                ReadingSessionErrors.NotFound(request.SessionId)
            );
        }

        if (session.UserId != userIdentifierProvider.UserId)
        {
            return Result.Failure<IReadOnlyList<ReadingSessionNoteDto>>(
                ReadingSessionErrors.NotOwned(request.SessionId)
            );
        }

        var notes = await readingSessionNotesQuery.GetBySessionIdAsync(
            request.SessionId,
            userIdentifierProvider.UserId,
            cancellationToken
        );

        return Result.Success<IReadOnlyList<ReadingSessionNoteDto>>(
            notes
                .Select(note => new ReadingSessionNoteDto(
                    note.Id.Value,
                    note.ChapterTitle,
                    note.Content,
                    note.CreatedOnUtc
                ))
                .ToList()
        );
    }
}
