using FluentAssertions;
using NSubstitute;
using TrackYourLife.Modules.Reading.Application.Features.ReadingSessions.Commands.LogReadingProgress;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;
using Xunit;

namespace TrackYourLife.Modules.Reading.Application.UnitTests.Features.ReadingSessions.Commands.LogReadingProgress;

public class LogReadingProgressCommandHandlerTests
{
    private readonly IBooksRepository _booksRepository;
    private readonly IBooksQuery _booksQuery;
    private readonly IReadingSessionsRepository _readingSessionsRepository;
    private readonly IReadingSessionsQuery _readingSessionsQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly LogReadingProgressCommandHandler _handler;
    private readonly UserId _userId = UserId.NewId();

    public LogReadingProgressCommandHandlerTests()
    {
        _booksRepository = Substitute.For<IBooksRepository>();
        _booksQuery = Substitute.For<IBooksQuery>();
        _readingSessionsRepository = Substitute.For<IReadingSessionsRepository>();
        _readingSessionsQuery = Substitute.For<IReadingSessionsQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _userIdentifierProvider.UserId.Returns(_userId);

        _handler = new LogReadingProgressCommandHandler(
            _booksRepository,
            _booksQuery,
            _readingSessionsRepository,
            _readingSessionsQuery,
            _userIdentifierProvider
        );
    }

    [Fact]
    public async Task Handle_WithOngoingBook_CreatesFinishedSessionAndIgnoresActiveSession()
    {
        var book = CreateOngoingBook(currentPage: 100, totalPages: 400);
        var bookReadModel = ToReadModel(book);

        _booksQuery
            .GetByUserIdAsync(
                _userId,
                BookStatus.Ongoing,
                BookSortField.StartingDate,
                true,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<BookReadModel> { bookReadModel });

        _readingSessionsQuery
            .GetFinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ReadingSessionReadModel>());

        _booksRepository.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);
        _readingSessionsRepository
            .GetMaxFinishedEndPageForBookAsync(book.Id, _userId, Arg.Any<CancellationToken>())
            .Returns((int?)null);

        var result = await _handler.Handle(
            new LogReadingProgressCommand(352, null),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.BookId.Should().Be(book.Id);
        result.Value.StartPage.Should().Be(100);
        result.Value.EndPage.Should().Be(352);
        result.Value.PagesRead.Should().Be(252);
        result.Value.BookTitle.Should().Be(book.Title);

        await _readingSessionsRepository
            .Received(1)
            .AddAsync(
                Arg.Is<ReadingSession>(session =>
                    session.BookId == book.Id
                    && session.StartPage == 100
                    && session.EndPage == 352
                    && session.FinishedOnUtc != null
                ),
                Arg.Any<CancellationToken>()
            );

        await _readingSessionsRepository
            .DidNotReceive()
            .GetActiveByUserIdAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PrefersOngoingBookFromMostRecentFinishedSession()
    {
        var olderBook = CreateOngoingBook(currentPage: 10, totalPages: 200, title: "Older");
        var recentBook = CreateOngoingBook(currentPage: 20, totalPages: 300, title: "Recent");

        _booksQuery
            .GetByUserIdAsync(
                _userId,
                BookStatus.Ongoing,
                BookSortField.StartingDate,
                true,
                Arg.Any<CancellationToken>()
            )
            .Returns(
                new List<BookReadModel> { ToReadModel(olderBook), ToReadModel(recentBook) }
            );

        var finished = new ReadingSessionReadModel(
            ReadingSessionId.NewId(),
            _userId,
            recentBook.Id,
            DateOnly.FromDateTime(DateTime.UtcNow),
            1,
            20,
            19,
            null,
            DateTime.UtcNow.AddDays(-1),
            DateTime.UtcNow.AddHours(-2),
            DateTime.UtcNow.AddDays(-1)
        );

        _readingSessionsQuery
            .GetFinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(new List<ReadingSessionReadModel> { finished });

        _booksRepository.GetByIdAsync(recentBook.Id, Arg.Any<CancellationToken>()).Returns(recentBook);
        _readingSessionsRepository
            .GetMaxFinishedEndPageForBookAsync(recentBook.Id, _userId, Arg.Any<CancellationToken>())
            .Returns(20);

        var result = await _handler.Handle(
            new LogReadingProgressCommand(50, null),
            CancellationToken.None
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.BookId.Should().Be(recentBook.Id);
        result.Value.BookTitle.Should().Be("Recent");
    }

    [Fact]
    public async Task Handle_WhenNoOngoingBook_ReturnsFailure()
    {
        _booksQuery
            .GetByUserIdAsync(
                _userId,
                BookStatus.Ongoing,
                BookSortField.StartingDate,
                true,
                Arg.Any<CancellationToken>()
            )
            .Returns(Array.Empty<BookReadModel>());

        var result = await _handler.Handle(
            new LogReadingProgressCommand(10, null),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.NoOngoingBook);
        await _readingSessionsRepository
            .DidNotReceive()
            .AddAsync(Arg.Any<ReadingSession>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenEndPageNotAfterCurrent_ReturnsFailure()
    {
        var book = CreateOngoingBook(currentPage: 352, totalPages: 400);

        _booksQuery
            .GetByUserIdAsync(
                _userId,
                BookStatus.Ongoing,
                BookSortField.StartingDate,
                true,
                Arg.Any<CancellationToken>()
            )
            .Returns(new List<BookReadModel> { ToReadModel(book) });

        _readingSessionsQuery
            .GetFinishedByUserIdAsync(_userId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<ReadingSessionReadModel>());

        _booksRepository.GetByIdAsync(book.Id, Arg.Any<CancellationToken>()).Returns(book);

        var result = await _handler.Handle(
            new LogReadingProgressCommand(352, null),
            CancellationToken.None
        );

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(BookErrors.EndPageNotAfterCurrent);
    }

    private Book CreateOngoingBook(int currentPage, int totalPages, string title = "Test Book")
    {
        return Book.Create(
                BookId.NewId(),
                _userId,
                title,
                "Author",
                totalPages,
                currentPage,
                BookStatus.Ongoing,
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7)),
                finishDate: null,
                rating: null
            )
            .Value;
    }

    private static BookReadModel ToReadModel(Book book) =>
        new(
            book.Id,
            book.UserId,
            book.Title,
            book.Author,
            book.TotalPages,
            book.CurrentPage,
            book.Status,
            book.StartingDate,
            book.FinishDate,
            book.Rating,
            book.CreatedOnUtc,
            book.ModifiedOnUtc
        );
}
