using TrackYourLife.Modules.Reading.Application.Services;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
using TrackYourLife.Modules.Reading.Domain.Features.Reading;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Application.UnitTests.Services;

public class ReadingStatisticsServiceTests
{
    private readonly ReadingStatisticsService _sut = new();

    [Fact]
    public void CalculateDailyProgress_AggregatesMultipleSessionsOnSameDay()
    {
        var userId = UserId.NewId();
        var bookId = BookId.NewId();
        var date = new DateOnly(2026, 6, 15);

        var sessions = new List<ReadingSessionReadModel>
        {
            CreateSession(userId, bookId, date, 10),
            CreateSession(userId, bookId, date, 5),
        };

        var result = _sut.CalculateDailyProgress(sessions, date, 20);

        result.PagesReadToday.Should().Be(15);
        result.Remaining.Should().Be(5);
        result.TargetMet.Should().BeFalse();
    }

    [Fact]
    public void CalculateStreak_CountsConsecutiveDays()
    {
        var userId = UserId.NewId();
        var bookId = BookId.NewId();
        var today = new DateOnly(2026, 6, 15);

        var sessions = new List<ReadingSessionReadModel>
        {
            CreateSession(userId, bookId, today, 20),
            CreateSession(userId, bookId, today.AddDays(-1), 20),
            CreateSession(userId, bookId, today.AddDays(-2), 5),
        };

        int? GetTarget(DateOnly date) => 20;

        var result = _sut.CalculateStreak(sessions, GetTarget, today);

        result.CurrentStreak.Should().Be(2);
        result.TodayTargetMet.Should().BeTrue();
    }

    [Fact]
    public void CalculatePagesHistory_AggregatesDailyPages()
    {
        var userId = UserId.NewId();
        var bookId = BookId.NewId();
        var startDate = new DateOnly(2026, 6, 10);
        var endDate = new DateOnly(2026, 6, 12);

        var sessions = new List<ReadingSessionReadModel>
        {
            CreateSession(userId, bookId, startDate, 10),
            CreateSession(userId, bookId, startDate, 5),
            CreateSession(userId, bookId, startDate.AddDays(1), 7),
        };

        var result = _sut.CalculatePagesHistory(
            sessions,
            startDate,
            endDate,
            ReadingOverviewType.Daily
        );

        result.Should().HaveCount(3);
        result[0].Pages.Should().Be(15);
        result[1].Pages.Should().Be(7);
        result[2].Pages.Should().Be(0);
    }

    [Fact]
    public void CalculatePagesHistory_AggregatesWeeklyPages()
    {
        var userId = UserId.NewId();
        var bookId = BookId.NewId();
        var monday = new DateOnly(2026, 6, 8);
        var wednesday = new DateOnly(2026, 6, 10);

        var sessions = new List<ReadingSessionReadModel>
        {
            CreateSession(userId, bookId, monday, 10),
            CreateSession(userId, bookId, wednesday, 8),
        };

        var result = _sut.CalculatePagesHistory(
            sessions,
            monday,
            monday.AddDays(6),
            ReadingOverviewType.Weekly
        );

        result.Should().ContainSingle();
        result[0].Pages.Should().Be(18);
    }

    [Fact]
    public void CalculatePagesHistory_AggregatesMonthlyPages()
    {
        var userId = UserId.NewId();
        var bookId = BookId.NewId();
        var juneStart = new DateOnly(2026, 6, 1);
        var juneMid = new DateOnly(2026, 6, 15);
        var julyStart = new DateOnly(2026, 7, 1);

        var sessions = new List<ReadingSessionReadModel>
        {
            CreateSession(userId, bookId, juneStart, 10),
            CreateSession(userId, bookId, juneMid, 5),
            CreateSession(userId, bookId, julyStart, 7),
        };

        var result = _sut.CalculatePagesHistory(
            sessions,
            juneStart,
            julyStart,
            ReadingOverviewType.Monthly
        );

        result.Should().HaveCount(2);
        result[0].Pages.Should().Be(15);
        result[1].Pages.Should().Be(7);
    }

    private static ReadingSessionReadModel CreateSession(
        UserId userId,
        BookId bookId,
        DateOnly date,
        int pagesRead
    ) =>
        new(
            ReadingSessionId.NewId(),
            userId,
            bookId,
            date,
            0,
            pagesRead,
            pagesRead,
            null,
            null,
            DateTime.UtcNow,
            DateTime.UtcNow,
            DateTime.UtcNow
        )
        {
            BookTitle = "Book",
            BookAuthor = "Author",
        };
}
