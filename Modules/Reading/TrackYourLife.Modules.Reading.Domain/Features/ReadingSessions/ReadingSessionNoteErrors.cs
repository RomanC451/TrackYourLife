using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public static class ReadingSessionNoteErrors
{
    public static readonly Func<ReadingSessionNoteId, Error> NotFound = id =>
        Error.NotFound(id, nameof(ReadingSessionNote));

    public static readonly Func<ReadingSessionNoteId, Error> NotOwned = id =>
        Error.NotOwned(id, nameof(ReadingSessionNote));

    public static readonly Func<ReadingSessionNoteId, Error> SessionMismatch = id =>
        new(
            "ReadingSessionNote.SessionMismatch",
            $"Note {id.Value} does not belong to the specified reading session.",
            400
        );
}
