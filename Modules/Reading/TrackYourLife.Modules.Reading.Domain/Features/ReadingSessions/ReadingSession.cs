using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public sealed class ReadingSession : AggregateRoot<ReadingSessionId>, IAuditableEntity
{
    public UserId UserId { get; private set; } = UserId.Empty;
    public BookId BookId { get; private set; } = BookId.Empty;
    public DateOnly? SessionDate { get; private set; }
    public int StartPage { get; private set; }
    public int? EndPage { get; private set; }
    public int? PagesRead { get; private set; }
    public int? DurationSeconds { get; private set; }
    public string? Notes { get; private set; }
    public DateTime StartedOnUtc { get; private set; }
    public DateTime? FinishedOnUtc { get; private set; }
    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; private set; }

    public bool IsActive => !FinishedOnUtc.HasValue;

    private ReadingSession() { }

    private ReadingSession(
        ReadingSessionId id,
        UserId userId,
        BookId bookId,
        int startPage,
        DateTime startedOnUtc
    )
        : base(id)
    {
        UserId = userId;
        BookId = bookId;
        StartPage = startPage;
        StartedOnUtc = startedOnUtc;
    }

    public static Result<ReadingSession> Start(
        ReadingSessionId id,
        UserId userId,
        BookId bookId,
        int startPage,
        DateTime startedOnUtc
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSession), nameof(id))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSession), nameof(userId))
            ),
            Ensure.NotEmptyId(
                bookId,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSession), nameof(bookId))
            ),
            Ensure.NotNegative(
                startPage,
                DomainErrors.ArgumentError.Negative(nameof(ReadingSession), nameof(startPage))
            ),
            Ensure.NotEmpty(
                startedOnUtc,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSession), nameof(startedOnUtc))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<ReadingSession>(result.Error);
        }

        return Result.Success(new ReadingSession(id, userId, bookId, startPage, startedOnUtc));
    }

    public Result Finish(
        int endPage,
        int totalPages,
        DateOnly sessionDate,
        DateTime finishedOnUtc,
        string? notes,
        int? durationSeconds
    )
    {
        if (!IsActive)
        {
            return Result.Failure(ReadingSessionErrors.SessionAlreadyFinished);
        }

        var validation = ValidatePages(StartPage, endPage, totalPages);

        if (validation.IsFailure)
        {
            return validation;
        }

        if (durationSeconds is < 0)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Negative(
                    nameof(ReadingSession),
                    nameof(durationSeconds)
                )
            );
        }

        EndPage = endPage;
        PagesRead = endPage - StartPage;
        SessionDate = sessionDate;
        FinishedOnUtc = finishedOnUtc;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        DurationSeconds = durationSeconds;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateFinished(
        int endPage,
        int totalPages,
        DateOnly sessionDate,
        string? notes,
        int? durationSeconds
    )
    {
        if (IsActive)
        {
            return Result.Failure(ReadingSessionErrors.SessionNotFinished);
        }

        var validation = ValidatePages(StartPage, endPage, totalPages);

        if (validation.IsFailure)
        {
            return validation;
        }

        if (durationSeconds is < 0)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Negative(
                    nameof(ReadingSession),
                    nameof(durationSeconds)
                )
            );
        }

        EndPage = endPage;
        PagesRead = endPage - StartPage;
        SessionDate = sessionDate;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        DurationSeconds = durationSeconds;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    private static Result ValidatePages(int startPage, int endPage, int totalPages)
    {
        if (endPage < startPage)
        {
            return Result.Failure(ReadingSessionErrors.EndPageBeforeStartPage);
        }

        if (endPage > totalPages)
        {
            return Result.Failure(ReadingSessionErrors.EndPageExceedsTotalPages);
        }

        return Result.Success();
    }
}
