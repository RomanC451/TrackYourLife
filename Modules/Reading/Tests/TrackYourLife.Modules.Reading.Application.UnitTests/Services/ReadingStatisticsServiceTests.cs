using TrackYourLife.Modules.Reading.Application.Services;
using TrackYourLife.Modules.Reading.Domain.Features.Books;
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
