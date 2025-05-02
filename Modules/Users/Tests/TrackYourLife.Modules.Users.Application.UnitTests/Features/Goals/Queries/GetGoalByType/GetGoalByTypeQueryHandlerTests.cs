using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetGoalByType;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Queries.GetGoalByType;

public sealed class GetGoalByTypeQueryHandlerTests
{
    private readonly IGoalQuery _goalQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IQueryHandler<GetGoalByTypeQuery, GoalReadModel> _handler;

    public GetGoalByTypeQueryHandlerTests()
    {
        _goalQuery = Substitute.For<IGoalQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetGoalByTypeQueryHandler(_goalQuery, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenGoalExists_ReturnsSuccess()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var type = GoalType.Calories;
        var value = 2000;

        var goal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: type,
            value: value,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var query = new GetGoalByTypeQuery(type, date);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalQuery
            .GetGoalByTypeAndDateAsync(userId, type, date, Arg.Any<CancellationToken>())
            .Returns(goal);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(goal);

        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(userId, type, date, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenGoalDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var type = GoalType.Calories;

        var query = new GetGoalByTypeQuery(type, date);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalQuery
            .GetGoalByTypeAndDateAsync(userId, type, date, Arg.Any<CancellationToken>())
            .Returns((GoalReadModel?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(GoalErrors.NotExisting(type));

        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(userId, type, date, Arg.Any<CancellationToken>());
    }
}
