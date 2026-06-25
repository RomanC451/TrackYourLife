using TrackYourLife.Modules.Reading.Application.Features.Reading.Queries.GetReadingPagesHistory;
using TrackYourLife.Modules.Reading.Application.Services;
using TrackYourLife.Modules.Reading.Contracts.Dtos;
using TrackYourLife.Modules.Reading.Domain.Features.Reading;
using TrackYourLife.Modules.Reading.Domain.Features.ReadingSessions;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Reading.Application.UnitTests.Features.Reading.Queries.GetReadingPagesHistory;

public class GetReadingPagesHistoryQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IReadingSessionsQuery _readingSessionsQuery;
    private readonly IReadingStatisticsService _readingStatisticsService;
    private readonly GetReadingPagesHistoryQueryHandler _handler;
    private readonly UserId _userId;

    public GetReadingPagesHistoryQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _readingSessionsQuery = Substitute.For<IReadingSessionsQuery>();
        _readingStatisticsService = Substitute.For<IReadingStatisticsService>();
        _handler = new GetReadingPagesHistoryQueryHandler(
            _readingSessionsQuery,
            _readingStatisticsService,
            _userIdentifierProvider
        );
        _userId = UserId.NewId();
        _userIdentifierProvider.UserId.Returns(_userId);
    }

    [Fact]
    public void ResolveDateRange_WhenExplicitDatesProvided_ShouldUseThem()
    {
        var start = new DateOnly(2026, 1, 1);
        var end = new DateOnly(2026, 1, 31);
        var today = new DateOnly(2026, 6, 15);

        var (resolvedStart, resolvedEnd) = GetReadingPagesHistoryQueryHandler.ResolveDateRange(
            new GetReadingPagesHistoryQuery(start, end, ReadingOverviewType.Weekly),
            today
        );

        resolvedStart.Should().Be(start);
        resolvedEnd.Should().Be(end);
    }

    [Fact]
    public void ResolveDateRange_Daily_ShouldReturnLast30Days()
    {
        var today = new DateOnly(2026, 6, 15);

        var (start, end) = GetReadingPagesHistoryQueryHandler.ResolveDateRange(
            new GetReadingPagesHistoryQuery(OverviewType: ReadingOverviewType.Daily),
            today
        );

        start.Should().Be(today.AddDays(-29));
        end.Should().Be(today);
    }

    [Fact]
    public void ResolveDateRange_Monthly_ShouldReturnLast12Months()
    {
        var today = new DateOnly(2026, 6, 15);

        var (start, end) = GetReadingPagesHistoryQueryHandler.ResolveDateRange(
            new GetReadingPagesHistoryQuery(OverviewType: ReadingOverviewType.Monthly),
            today
        );

        start.Should().Be(new DateOnly(2025, 7, 1));
        end.Should().Be(today);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagesHistoryFromStatisticsService()
    {
        var start = new DateOnly(2026, 6, 1);
        var end = new DateOnly(2026, 6, 7);
        var sessions = new List<ReadingSessionReadModel>();
        var history = new List<ReadingPagesDataPointDto>
        {
            new(start, 10, start, start),
        };

        _readingSessionsQuery
            .GetFinishedByUserIdAndDateRangeAsync(
                _userId,
                start,
                end,
                Arg.Any<CancellationToken>()
            )
            .Returns(sessions);

        _readingStatisticsService
            .CalculatePagesHistory(sessions, start, end, ReadingOverviewType.Weekly)
            .Returns(history);

        var result = await _handler.Handle(
            new GetReadingPagesHistoryQuery(start, end, ReadingOverviewType.Weekly),
            default
        );

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(history);
    }
}
