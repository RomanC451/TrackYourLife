using TrackYourLife.Modules.Reading.Application.Abstraction;
using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetDailyReadingProgress;
using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingStreak;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Contracts.MappingsExtensions;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;
using MediatR;

namespace TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingDashboard;

internal sealed class GetReadingDashboardQueryHandler(
    ISender sender,
    IBooksQuery booksQuery,
    IReadingSessionsQuery readingSessionsQuery,
    IUserIdentifierProvider userIdentifierProvider
) : IQueryHandler<GetReadingDashboardQuery, ReadingDashboardDto>
{
    public async Task<Result<ReadingDashboardDto>> Handle(
        GetReadingDashboardQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = userIdentifierProvider.UserId;

        var streakResult = await sender.Send(new GetReadingStreakQuery(), cancellationToken);
        if (streakResult.IsFailure)
        {
            return Result.Failure<ReadingDashboardDto>(streakResult.Error);
        }

        var progressResult = await sender.Send(
            new GetDailyReadingProgressQuery(null),
            cancellationToken
        );
        if (progressResult.IsFailure)
        {
            return Result.Failure<ReadingDashboardDto>(progressResult.Error);
        }

        var activeSession = await readingSessionsQuery.GetActiveByUserIdAsync(
            userId,
            cancellationToken
        );

        var books = await booksQuery.GetByUserIdAsync(
            userId,
            status: BookStatus.Ongoing,
            BookSortField.StartingDate,
            sortDescending: true,
            cancellationToken
        );

        var recentBooks = books.Take(5).ToList();

        var dashboard = new ReadingDashboardDto(
            streakResult.Value,
            progressResult.Value,
            activeSession is null ? null : MapSession(activeSession),
            recentBooks.Select(MapBook).ToList()
        );

        return Result.Success(dashboard);
    }

    private static BookDto MapBook(BookReadModel book) =>
        new(
            book.Id.Value,
            book.Title,
            book.Author,
            book.TotalPages,
            book.CurrentPage,
            book.Status,
            book.StartingDate,
            book.FinishDate,
            book.Rating,
            book.CreatedOnUtc,
            book.ModifiedOnUtc,
            book.SuggestMarkAsFinished()
        );

    private static ReadingSessionDto MapSession(ReadingSessionReadModel session) =>
        new(
            session.Id.Value,
            session.BookId.Value,
            session.BookTitle,
            session.BookAuthor,
            session.SessionDate,
            session.StartPage,
            session.EndPage,
            session.PagesRead,
            session.DurationSeconds,
            session.StartedOnUtc,
            session.FinishedOnUtc,
            session.CreatedOnUtc,
            session.IsActive
        );
}
