using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using TrackYourLife.Modules.Users.Application.Features.Goals.Queries.GetNutritionGoals;
using TrackYourLife.Modules.Users.Contracts.Dtos;
using TrackYourLife.Modules.Users.Domain.Features.Goals;
using TrackYourLife.Modules.Users.Presentation.Features.Goals.Queries;
using TrackYourLife.SharedLib.Domain.Enums;
using TrackYourLife.SharedLib.Domain.Errors;
using TrackYourLife.SharedLib.Domain.Ids;
using TrackYourLife.SharedLib.Domain.Results;

namespace TrackYourLife.Modules.Users.Presentation.UnitTests.Features.Goals.Queries;

public class GetNutritionGoalsTests
{
    private readonly ISender _sender;
    private readonly GetNutritionGoals _endpoint;

    public GetNutritionGoalsTests()
    {
        _sender = Substitute.For<ISender>();
        _endpoint = new GetNutritionGoals(_sender);
    }

    [Fact]
    public async Task ExecuteAsync_WhenQuerySucceeds_ShouldReturnOkWithGoalDtoList()
    {
        // Arrange
        var userId = UserId.NewId();
        var date = new DateOnly(2024, 1, 15);

        var goals = new List<GoalReadModel>
        {
            new(
                Id: GoalId.NewId(),
                UserId: userId,
                Value: 2000,
                Type: GoalType.Calories,
                Period: GoalPeriod.Day,
                StartDate: new DateOnly(2024, 1, 1),
                EndDate: new DateOnly(2024, 1, 31)
            ),
            new(
                Id: GoalId.NewId(),
                UserId: userId,
                Value: 150,
                Type: GoalType.Protein,
                Period: GoalPeriod.Day,
                StartDate: new DateOnly(2024, 1, 1),
                EndDate: new DateOnly(2024, 1, 31)
            ),
            new(
                Id: GoalId.NewId(),
                UserId: userId,
                Value: 250,
                Type: GoalType.Carbohydrates,
                Period: GoalPeriod.Day,
                StartDate: new DateOnly(2024, 1, 1),
                EndDate: new DateOnly(2024, 1, 31)
            ),
            new(
                Id: GoalId.NewId(),
                UserId: userId,
                Value: 65,
                Type: GoalType.Fats,
                Period: GoalPeriod.Day,
                StartDate: new DateOnly(2024, 1, 1),
                EndDate: new DateOnly(2024, 1, 31)
            )
        };

        var request = new GetNutritionGoalsRequest
        {
            Date = date
        };

        _sender
            .Send(Arg.Any<GetNutritionGoalsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Success<List<GoalReadModel>>(goals)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Should().BeOfType<Ok<List<GoalDto>>>().Subject;
        okResult.Value.Should().NotBeNull();
        var valueList = okResult.Value!;
        valueList.Should().HaveCount(4);
        valueList[0].Type.Should().Be(GoalType.Calories);
        valueList[1].Type.Should().Be(GoalType.Protein);

        await _sender
            .Received(1)
            .Send(
                Arg.Is<GetNutritionGoalsQuery>(q => q.Date == date),
                Arg.Any<CancellationToken>()
            );
    }

    [Fact]
    public async Task ExecuteAsync_WhenQueryFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new GetNutritionGoalsRequest
        {
            Date = new DateOnly(2024, 1, 15)
        };

        var error = new Error("Goals", "Nutrition goals not found");
        _sender
            .Send(Arg.Any<GetNutritionGoalsQuery>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(Result.Failure<List<GoalReadModel>>(error)));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequest<ProblemDetails>>();
    }
}
