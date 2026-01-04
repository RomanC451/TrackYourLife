using TrackYourLife.Modules.Youtube.Application.Features.DailyEntertainmentCounters.Queries.GetDailyEntertainmentCounter;
using TrackYourLife.Modules.Youtube.Domain.Features.DailyEntertainmentCounters;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Youtube.Application.UnitTests.Features.DailyEntertainmentCounters.Queries.GetDailyEntertainmentCounter;

public sealed class GetDailyEntertainmentCounterQueryHandlerTests
{
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IDailyEntertainmentCountersRepository _dailyEntertainmentCountersRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly GetDailyEntertainmentCounterQueryHandler _handler;

    public GetDailyEntertainmentCounterQueryHandlerTests()
    {
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _dailyEntertainmentCountersRepository =
            Substitute.For<IDailyEntertainmentCountersRepository>();
        _dateTimeProvider = Substitute.For<IDateTimeProvider>();

        _handler = new GetDailyEntertainmentCounterQueryHandler(
            _userIdentifierProvider,
            _dailyEntertainmentCountersRepository,
            _dateTimeProvider
        );
    }

    [Fact]
    public async Task Handle_WhenCounterExists_ReturnsCounter()
    {
        // Arrange
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(utcNow);
        var query = new GetDailyEntertainmentCounterQuery();

        var counter = DailyEntertainmentCounter
            .Create(DailyEntertainmentCounterId.NewId(), userId, today, 3)
            .Value;

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _dailyEntertainmentCountersRepository
            .GetByUserIdAndDateAsync(userId, today, Arg.Any<CancellationToken>())
            .Returns(counter);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.UserId.Should().Be(userId);
        result.Value.Date.Should().Be(today);
        result.Value.VideosWatchedCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_WhenCounterDoesNotExist_ReturnsNull()
    {
        // Arrange
        var userId = UserId.NewId();
        var utcNow = DateTime.UtcNow;
        var today = DateOnly.FromDateTime(utcNow);
        var query = new GetDailyEntertainmentCounterQuery();

        _userIdentifierProvider.UserId.Returns(userId);
        _dateTimeProvider.UtcNow.Returns(utcNow);
        _dailyEntertainmentCountersRepository
            .GetByUserIdAndDateAsync(userId, today, Arg.Any<CancellationToken>())
            .Returns((DailyEntertainmentCounter?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeNull();
    }
}
