using TrackYourLife.Modules.Users.Application.Core.Abstraction.Messaging;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Domain.UnitTests.Utils;
using TrackYourLife.SharedLib.Application.Abstraction;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Ids;

namespace TrackYourLife.Modules.Users.Application.UnitTests.Features.Goals.Queries.GetNutritionGoals;

public sealed class GetNutritionGoalsQueryHandlerTests : IDisposable
{
    private readonly IGoalQuery _goalQuery;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IQueryHandler<GetNutritionGoalsQuery, List<GoalReadModel>> _handler;

    public GetNutritionGoalsQueryHandlerTests()
    {
        _goalQuery = Substitute.For<IGoalQuery>();
        _userIdentifierProvider = Substitute.For<IUserIdentifierProvider>();
        _handler = new GetNutritionGoalsQueryHandler(_goalQuery, _userIdentifierProvider);
    }

    [Fact]
    public async Task Handle_WhenAllGoalsExist_ReturnsAllGoals()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var caloriesGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Calories,
            value: 2000,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var proteinGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Protein,
            value: 150,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var carbsGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Carbohydrates,
            value: 250,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var fatsGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Fats,
            value: 70,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var query = new GetNutritionGoalsQuery(date);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns(caloriesGoal);
        _goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Protein, date, Arg.Any<CancellationToken>())
            .Returns(proteinGoal);
        _goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns(carbsGoal);
        _goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>())
            .Returns(fatsGoal);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(4);
        result.Value.Should().Contain(caloriesGoal);
        result.Value.Should().Contain(proteinGoal);
        result.Value.Should().Contain(carbsGoal);
        result.Value.Should().Contain(fatsGoal);

        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            );
        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Protein,
                date,
                Arg.Any<CancellationToken>()
            );
        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            );
        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenSomeGoalsAreMissing_ReturnsEmptyList()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);

        var caloriesGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Calories,
            value: 2000,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var proteinGoal = GoalFaker.GenerateReadModel(
            userId: userId,
            type: GoalType.Protein,
            value: 150,
            startDate: date,
            endDate: date.AddDays(1)
        );

        var query = new GetNutritionGoalsQuery(date);

        _userIdentifierProvider.UserId.Returns(userId);
        _goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns(caloriesGoal);
        _goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Protein, date, Arg.Any<CancellationToken>())
            .Returns(proteinGoal);
        _goalQuery
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            )
            .Returns((GoalReadModel?)null);
        _goalQuery
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>())
            .Returns((GoalReadModel?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Calories,
                date,
                Arg.Any<CancellationToken>()
            );
        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Protein,
                date,
                Arg.Any<CancellationToken>()
            );
        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(
                userId,
                GoalType.Carbohydrates,
                date,
                Arg.Any<CancellationToken>()
            );
        await _goalQuery
            .Received(1)
            .GetGoalByTypeAndDateAsync(userId, GoalType.Fats, date, Arg.Any<CancellationToken>());
    }

    public void Dispose()
    {
        _goalQuery.ClearSubstitute();
        _userIdentifierProvider.ClearSubstitute();
        GC.SuppressFinalize(this);
    }
}
