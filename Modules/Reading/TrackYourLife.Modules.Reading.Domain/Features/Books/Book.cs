using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Primitives;
using TrackYourLife.SharedLib.Domain.Results;
using TrackYourLife.SharedLib.Domain.Utils;

namespace TrackYourLife.Modules.Reading.Domain.Features.Books;

public sealed class Book : AggregateRoot<BookId>, IAuditableEntity
{
    public UserId UserId { get; private set; } = UserId.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Author { get; private set; } = string.Empty;
    public int TotalPages { get; private set; }
    public int CurrentPage { get; private set; }
    public BookStatus Status { get; private set; }
    public DateOnly? StartingDate { get; private set; }
    public DateOnly? FinishDate { get; private set; }
    public int? Rating { get; private set; }
    public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? ModifiedOnUtc { get; private set; }

    private Book() { }

    private Book(
        BookId id,
        UserId userId,
        string title,
        string author,
        int totalPages,
        int currentPage,
        BookStatus status,
        DateOnly? startingDate,
        DateOnly? finishDate,
        int? rating
    )
        : base(id)
    {
        UserId = userId;
        Title = title;
        Author = author;
        TotalPages = totalPages;
        CurrentPage = currentPage;
        Status = status;
        StartingDate = startingDate;
        FinishDate = finishDate;
        Rating = rating;
    }

    public static Result<Book> Create(
        BookId id,
        UserId userId,
        string title,
        string author,
        int totalPages,
        int currentPage,
        BookStatus status,
        DateOnly? startingDate,
        DateOnly? finishDate,
        int? rating
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmptyId(id, DomainErrors.ArgumentError.Empty(nameof(Book), nameof(id))),
            Ensure.NotEmptyId(
                userId,
                DomainErrors.ArgumentError.Empty(nameof(Book), nameof(userId))
            ),
            Ensure.NotEmpty(title, DomainErrors.ArgumentError.Empty(nameof(Book), nameof(title))),
            Ensure.NotEmpty(
                author,
                DomainErrors.ArgumentError.Empty(nameof(Book), nameof(author))
            ),
            Ensure.Positive(
                totalPages,
                DomainErrors.ArgumentError.NotPositive(nameof(Book), nameof(totalPages))
            ),
            Ensure.NotNegative(
                currentPage,
                DomainErrors.ArgumentError.Negative(nameof(Book), nameof(currentPage))
            ),
            Ensure.IsInEnum(
                status,
                DomainErrors.ArgumentError.Invalid(nameof(Book), nameof(status))
            )
        );

        if (result.IsFailure)
        {
            return Result.Failure<Book>(result.Error);
        }

        if (currentPage > totalPages)
        {
            return Result.Failure<Book>(
                DomainErrors.ArgumentError.Custom(
                    nameof(Book),
                    nameof(currentPage),
                    "Current page cannot exceed total pages."
                )
            );
        }

        var statusValidation = ValidateStatusFields(
            status,
            startingDate,
            finishDate,
            rating
        );

        if (statusValidation.IsFailure)
        {
            return Result.Failure<Book>(statusValidation.Error);
        }

        return Result.Success(
            new Book(
                id,
                userId,
                title.Trim(),
                author.Trim(),
                totalPages,
                currentPage,
                status,
                startingDate,
                finishDate,
                rating
            )
        );
    }

    public Result Update(
        string title,
        string author,
        int totalPages,
        int currentPage,
        BookStatus status,
        DateOnly? startingDate,
        DateOnly? finishDate,
        int? rating
    )
    {
        var result = Result.FirstFailureOrSuccess(
            Ensure.NotEmpty(title, DomainErrors.ArgumentError.Empty(nameof(Book), nameof(title))),
            Ensure.NotEmpty(
                author,
                DomainErrors.ArgumentError.Empty(nameof(Book), nameof(author))
            ),
            Ensure.Positive(
                totalPages,
                DomainErrors.ArgumentError.NotPositive(nameof(Book), nameof(totalPages))
            ),
            Ensure.NotNegative(
                currentPage,
                DomainErrors.ArgumentError.Negative(nameof(Book), nameof(currentPage))
            ),
            Ensure.IsInEnum(
                status,
                DomainErrors.ArgumentError.Invalid(nameof(Book), nameof(status))
            )
        );

        if (result.IsFailure)
        {
            return result;
        }

        if (currentPage > totalPages)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Custom(
                    nameof(Book),
                    nameof(currentPage),
                    "Current page cannot exceed total pages."
                )
            );
        }

        var statusValidation = ValidateStatusFields(
            status,
            startingDate,
            finishDate,
            rating
        );

        if (statusValidation.IsFailure)
        {
            return statusValidation;
        }

        Title = title.Trim();
        Author = author.Trim();
        TotalPages = totalPages;
        CurrentPage = currentPage;
        Status = status;
        StartingDate = startingDate;
        FinishDate = finishDate;
        Rating = rating;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public Result UpdateCurrentPage(int currentPage)
    {
        var result = Ensure.NotNegative(
            currentPage,
            DomainErrors.ArgumentError.Negative(nameof(Book), nameof(currentPage))
        );

        if (result.IsFailure)
        {
            return result;
        }

        if (currentPage > TotalPages)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Custom(
                    nameof(Book),
                    nameof(currentPage),
                    "Current page cannot exceed total pages."
                )
            );
        }

        CurrentPage = currentPage;
        ModifiedOnUtc = DateTime.UtcNow;

        return Result.Success();
    }

    public bool SuggestMarkAsFinished() => CurrentPage >= TotalPages;

    public Result BeginReading(DateOnly startingDate)
    {
        if (Status == BookStatus.Ongoing)
        {
            return Result.Success();
        }

        if (Status == BookStatus.NotStarted)
        {
            Status = BookStatus.Ongoing;
            StartingDate = startingDate;
            ModifiedOnUtc = DateTime.UtcNow;
            return Result.Success();
        }

        if (Status == BookStatus.Finished)
        {
            Status = BookStatus.Ongoing;
            StartingDate ??= startingDate;
            FinishDate = null;
            Rating = null;
            ModifiedOnUtc = DateTime.UtcNow;
            return Result.Success();
        }

        return Result.Success();
    }

    private static Result ValidateStatusFields(
        BookStatus status,
        DateOnly? startingDate,
        DateOnly? finishDate,
        int? rating
    )
    {
        if (status is BookStatus.Ongoing or BookStatus.Finished && startingDate is null)
        {
            return Result.Failure(
                DomainErrors.ArgumentError.Custom(
                    nameof(Book),
                    nameof(startingDate),
                    "Starting date is required when status is Ongoing or Finished."
                )
            );
        }

        if (status == BookStatus.Finished)
        {
            if (finishDate is null)
            {
                return Result.Failure(
                    DomainErrors.ArgumentError.Custom(
                        nameof(Book),
                        nameof(finishDate),
                        "Finish date is required when status is Finished."
                    )
                );
            }

            if (rating is null or < 1 or > 5)
            {
                return Result.Failure(
                    DomainErrors.ArgumentError.Custom(
                        nameof(Book),
                        nameof(rating),
                        "Rating must be between 1 and 5 when status is Finished."
                    )
                );
            }
        }
        else
        {
            if (finishDate is not null)
            {
                return Result.Failure(
                    DomainErrors.ArgumentError.Custom(
                        nameof(Book),
                        nameof(finishDate),
                        "Finish date can only be set when status is Finished."
                    )
                );
            }

            if (rating is not null)
            {
                return Result.Failure(
                    DomainErrors.ArgumentError.Custom(
                        nameof(Book),
                        nameof(rating),
                        "Rating can only be set when status is Finished."
                    )
                );
            }
        }

        return Result.Success();
    }
}
