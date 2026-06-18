using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public static class ReadingSessionErrors
{
    public static readonly Func<ReadingSessionId, Error> NotFound = id =>
        Error.NotFound(id, nameof(ReadingSession));

    public static readonly Func<ReadingSessionId, Error> NotOwned = id =>
        Error.NotOwned(id, nameof(ReadingSession));

    public static readonly Error ActiveSessionExists = new(
        "ReadingSession.ActiveSessionExists",
        "You already have an active reading session.",
        409
    );

    public static readonly Error NoActiveSession = new(
        "ReadingSession.NoActiveSession",
        "No active reading session was found.",
        404
    );

    public static readonly Error SessionAlreadyFinished = new(
        "ReadingSession.AlreadyFinished",
        "This reading session has already been finished.",
        400
    );

    public static readonly Error SessionNotFinished = new(
        "ReadingSession.NotFinished",
        "This reading session is still active.",
        400
    );

    public static readonly Error EndPageBeforeStartPage = new(
        "ReadingSession.EndPageBeforeStartPage",
        "End page cannot be lower than start page.",
        400
    );

    public static readonly Error EndPageExceedsTotalPages = new(
        "ReadingSession.EndPageExceedsTotalPages",
        "End page cannot exceed the book's total pages.",
        400
    );
}
