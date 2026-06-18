using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;

public sealed class ReadingSessionNote : Entity<ReadingSessionNoteId>
{
    public const int MaxChapterTitleLength = 200;
    public const int MaxContentLength = 4000;

    public ReadingSessionId ReadingSessionId { get; private set; } = ReadingSessionId.Empty;
    public BookId BookId { get; private set; } = BookId.Empty;
    public UserId UserId { get; private set; } = UserId.Empty;
    public string ChapterTitle { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public DateTime CreatedOnUtc { get; private set; }

    private ReadingSessionNote() { }

    private ReadingSessionNote(
        ReadingSessionNoteId id,
        ReadingSessionId readingSessionId,
        BookId bookId,
        UserId userId,
        string chapterTitle,
        string content,
        DateTime createdOnUtc
    )
        : base(id)
    {
        ReadingSessionId = readingSessionId;
        BookId = bookId;
        UserId = userId;
        ChapterTitle = chapterTitle;
        Content = content;
        CreatedOnUtc = createdOnUtc;
    }

    public static Result<ReadingSessionNote> Create(
        ReadingSessionNoteId id,
        ReadingSessionId readingSessionId,
        BookId bookId,
        UserId userId,
        string chapterTitle,
        string content,
        DateTime createdOnUtc
    )
    {
        var trimmedChapterTitle = chapterTitle.Trim();
        var trimmedContent = content.Trim();

        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(
                id,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSessionNote), nameof(id))
            ),
            Ensure.NotEmptyId(
                readingSessionId,
                DomainErrors.ArgumentError.Empty(
                    nameof(ReadingSessionNote),
                    nameof(readingSessionId)
                )
            ),
            Ensure.NotEmptyId(
                bookId,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSessionNote), nameof(bookId))
            ),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSessionNote), nameof(userId))
            ),
            Ensure.NotEmpty(
                trimmedChapterTitle,
                DomainErrors.ArgumentError.Empty(
                    nameof(ReadingSessionNote),
                    nameof(chapterTitle)
                )
            ),
            Ensure.NotEmpty(
                trimmedContent,
                DomainErrors.ArgumentError.Empty(nameof(ReadingSessionNote), nameof(content))
            ),
            Ensure.IsTrue(
                trimmedChapterTitle.Length <= MaxChapterTitleLength,
                DomainErrors.ArgumentError.Invalid(
                    nameof(ReadingSessionNote),
                    nameof(chapterTitle)
                )
            ),
            Ensure.IsTrue(
                trimmedContent.Length <= MaxContentLength,
                DomainErrors.ArgumentError.Invalid(nameof(ReadingSessionNote), nameof(content))
            ),
            Ensure.NotEmpty(
                createdOnUtc,
                DomainErrors.ArgumentError.Empty(
                    nameof(ReadingSessionNote),
                    nameof(createdOnUtc)
                )
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<ReadingSessionNote>(result.Error);
        }

        return Result.Success(
            new ReadingSessionNote(
                id,
                readingSessionId,
                bookId,
                userId,
                trimmedChapterTitle,
                trimmedContent,
                createdOnUtc
            )
        );
    }
}
