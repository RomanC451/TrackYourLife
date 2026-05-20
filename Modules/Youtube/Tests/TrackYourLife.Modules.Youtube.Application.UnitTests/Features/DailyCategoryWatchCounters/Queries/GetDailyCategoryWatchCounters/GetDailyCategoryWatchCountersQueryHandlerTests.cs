using TrackYourLife.Modules.Youtube.Application.Features.DailyCategoryWatchCounters.Queries.GetDailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyCategoryWatchCounters;
using TrackYourLife.Modules.Youtube.Domain.Features.YoutubeCategories;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.DailyCategoryWatchCounters.Queries.GetDailyCategoryWatchCounters;

public sealed class GetDailyCategoryWatchCountersQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDailyCategoryWatchCountersQuery _dailyCategoryWatchCountersQuery;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly GetDailyCategoryWatchCountersQueryHandler _handler;

    public GetDailyCategoryWatchCountersQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _dailyCategoryWatchCountersQuery = Substitute.For<IDailyCategoryWatchCountersQuery>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new GetDailyCategoryWatchCountersQueryHandler(
            _userIdentifierProvider,
            _dailyCategoryWatchCountersQuery,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_ReturnsRowsForToday()
    {
        var userId = UserId.NewId();
        var utc = new DateTime(2026, 5, 14, 12, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(utc);
        var catId = YoutubeCategoryId.NewId();
        var rows = new List<DailyCategoryWatchCounterReadModel>
        {
            new(
                DailyCategoryWatchCounterId.NewId(),
                userId,
                today,
                catId,
                VideosWatchedCount: 2
            ),
        };

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utc);
        _dailyCategoryWatchCountersQuery
            .ListByUserIdAndDateAsync(userId, today, Arg.Any<CancellationToken>())
            .Returns(rows);

        var result = await _handler.Handle(new GetDailyCategoryWatchCountersQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].VideosWatchedCount.Should().Be(2);
    }
}
