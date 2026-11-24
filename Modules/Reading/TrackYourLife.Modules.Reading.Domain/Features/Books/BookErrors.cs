using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Reading.Domain.Features.Books;

public static class BookErrors
{
    public static readonly Func<BookId, Error> NotFound = id => Error.NotFound(id, nameof(Book));
    public static readonly Func<BookId, Error> NotOwned = id => Error.NotOwned(id, nameof(Book));

    public static readonly Error ActiveSessionExists = new(
        "Book.ActiveSessionExists",
        "You already have an active reading session.",
        409
    );

    public static readonly Error InvalidStatusTransition = new(
        "Book.InvalidStatusTransition",
        "The book status transition is invalid.",
        400
    );

    public static readonly Error NoOngoingBook = new(
        "Book.NoOngoingBook",
        "No ongoing book was found to log reading progress against.",
        404
    );

    public static readonly Error EndPageNotAfterCurrent = new(
        "Book.EndPageNotAfterCurrent",
        "End page must be greater than the book's current page.",
        400
    );
}
