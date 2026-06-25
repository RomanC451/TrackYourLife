using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

namespace TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Queries.GetReadingSessionNotes;

public sealed record GetReadingSessionNotesQuery(ReadingSessionId SessionId)
    : IQuery<IReadOnlyList<ReadingSessionNoteDto>>;
